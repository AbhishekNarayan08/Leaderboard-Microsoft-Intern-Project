// <copyright file="BulkActivityService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace BulkActivityProcessor
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Azure.Storage.Blobs.Models;
    using AzureStorage;
    using Common.Constants;
    using Common.Models.Requests;
    using Common.Models.Responses;
    using Common.Utils;
    using Configs;
    using CosmosDB.Interfaces;
    using CosmosDB.Models;
    using CosmosDB.Util;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using RuleEngine.Core;

    public class BulkActivityService : IBulkActivityService
    {
        private readonly IBlobStorageContext blobStorageContext;
        private readonly ICosmosDbReadClient cosmosDbReadClient;
        private readonly ICosmosDbWriteClient cosmosDbWriteClient;
        private readonly IRuleEngine ruleEngine;
        private readonly IAzureStorageConfig azureStorageConfig;

        public BulkActivityService(
            IBlobStorageContext blobStorageContext,
            ICosmosDbReadClient cosmosDbReadClient,
            ICosmosDbWriteClient cosmosDbWriteClient,
            IRuleEngine ruleEngine,
            IAzureStorageConfig azureStorageConfig)
        {
            this.blobStorageContext = blobStorageContext;
            this.cosmosDbReadClient = cosmosDbReadClient;
            this.cosmosDbWriteClient = cosmosDbWriteClient;
            this.ruleEngine = ruleEngine;
            this.azureStorageConfig = azureStorageConfig;
        }

        public List<ActivityRequest> GetValidateActivities(List<string> events, ILogger log)
        {
            var activities = new List<ActivityRequest>();

            if (events.Count == 0)
            {
                log.LogError("Event count is zero");
                return activities;
            }

            foreach (var eventData in events)
            {
                var actityRequest = JsonConvertUtil.DeserializeActivityRequest(eventData);
                if (actityRequest != null && actityRequest.ValidateData() == ResponseCode.SUCCESS)
                {
                    activities.Add(actityRequest);
                }
                else
                {
                    log.LogWarning("Invalid activity request.");
                }
            }

            return activities;
        }

        public async Task<List<ActivityRequest>> MoveActivityBlobs(
            List<ActivityRequest> activityRequests,
            string sourceContainer,
            string destinationContainer,
            ILogger logger)
        {
            // TODO: upload blobs in batches.
            int moveCount = 0;
            List<ActivityRequest> newActivityRequests = new List<ActivityRequest>();
            foreach (var activityRequest in activityRequests)
            {
                var response = await this.blobStorageContext.UploadAddActivityRequest(
                    activityRequest, destinationContainer, logger);
                if (response == ResponseCode.SUCCESS || response == ResponseCode.ACTIVITY_ID_ALREADY_EXISTS)
                {
                    if (response == ResponseCode.SUCCESS)
                    {
                        newActivityRequests.Add(activityRequest);
                    }

                    moveCount += await this.blobStorageContext.DeleteBlob(sourceContainer, activityRequest.ActivityId) ? 1 : 0;
                }
            }

            return newActivityRequests;
        }

        public async Task<IDictionary<UserProfile, List<ReputationActivity>>> ApplyRulesOnActivities(
            List<ActivityRequest> actityRequests,
            ILogger log)
        {
            var activitiesByUserId = actityRequests.GroupBy(x => x.UserId);
            IDictionary<UserProfile, List<ReputationActivity>> userProfileActivities = new Dictionary<UserProfile, List<ReputationActivity>>();

            foreach (var userActivities in activitiesByUserId)
            {
                var userId = userActivities.Key;
                var userProfile = await CosmosDbReadUtility.GetUserReputation(this.cosmosDbReadClient, userId);
                List<ReputationActivity> reputationActivities = new List<ReputationActivity>();

                foreach (var actityRequest in userActivities)
                {
                    var activity = actityRequest.ToReputationActivity();
                    userProfile ??= actityRequest.ToUserProfile();
                    reputationActivities.Add(activity);
                }

                await this.ruleEngine.BatchProcess(
                        userProfile: userProfile,
                        activities: reputationActivities,
                        logger: log);

                userProfileActivities.Add(userProfile, reputationActivities);
            }

            return userProfileActivities;
        }

        public async Task<int> UpdateBatchActivitiesInDb(
            KeyValuePair<UserProfile, List<ReputationActivity>> userProfileActivities,
            ExecutionContext context,
            ILogger logger)
        {
            if (userProfileActivities.Value != null &&
                userProfileActivities.Value.Any())
            {
                if (await this.cosmosDbWriteClient.BulkWriteAsync(
                    context: context,
                    partitionKey: "/userId",
                    userProfileDoc: userProfileActivities.Key,
                    activityDocs: userProfileActivities.Value))
                {
                    return 1;
                }
            }

            return 0;
        }

        public async Task<bool> ProcessActivitiesInBatches(
            ExecutionContext context,
            string containerName,
            ILogger logger)
        {
            if (string.IsNullOrEmpty(containerName))
            {
                return false;
            }

            try
            {
                int segmentSize = this.azureStorageConfig.BlobsBatchSize;
                if (segmentSize <= 0)
                {
                    logger.LogWarning($"Invalid batch size {segmentSize}");
                    return false;
                }

                var resultSegment = this.blobStorageContext.GetBlobItemPages(containerName, segmentSize);

                // Enumerate the blobs returned for each page.
                await foreach (Azure.Page<BlobItem> blobPage in resultSegment)
                {
                    List<ActivityRequest> activityRequests = new List<ActivityRequest>();
                    foreach (BlobItem blobItem in blobPage.Values)
                    {
                        if (blobItem != null && !blobItem.Deleted)
                        {
                            // download blob content async and serialize to ActivityRequest.
                            var activityRequest =
                                await this.blobStorageContext.DownloadJSONObjectAsync<ActivityRequest>(
                                    containerName, blobItem.Name);

                            // discard invalid activities & store valid activities.
                            if (activityRequest != null && activityRequest.ValidateData() == ResponseCode.SUCCESS)
                            {
                                activityRequests.Add(activityRequest);
                            }
                        }
                    }

                    // process activityRequests batch.
                    if (activityRequests.Any())
                    {
                        // move activity requests to ApiConstants.ActivityStorageContainerName container.
                        var newActivityRequests = await this.MoveActivityBlobs(
                                activityRequests: activityRequests,
                                sourceContainer: ApiConstants.SegmentOnboardActivitiesContainerName,
                                destinationContainer: ApiConstants.ActivityStorageContainerName,
                                logger: logger);

                        // apply rule engine rules and update activity data with repuptation score/badge.
                        var processedActivityData = await this.ApplyRulesOnActivities(newActivityRequests, logger);

                        int successCount = 0;
                        foreach (var userActivityData in processedActivityData)
                        {
                            // update bulk activities of userprofile.
                            successCount += await this.UpdateBatchActivitiesInDb(userActivityData, context, logger);
                        }

                        logger.LogInformation($"{successCount} db tasks successed out of {processedActivityData.Count}.");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
