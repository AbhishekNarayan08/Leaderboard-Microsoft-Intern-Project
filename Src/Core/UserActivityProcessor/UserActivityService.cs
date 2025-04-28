// <copyright file="UserActivityService.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace UserActivityProcessor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using AzureStorage;
    using Common.Models.Requests;
    using Common.Models.Responses;
    using Common.Utils;
    using CosmosDB.Interfaces;
    using CosmosDB.Services.Data;
    using CosmosDB.Util;
    using EventHub;
    using LeaderboardProcessor;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.EventHubs;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using RuleEngine.Core;

    public class UserActivityService : IUserActivityService
    {
        private readonly IBlobStorageContext blobStorageContext;
        private readonly IEventHubContext eventhubContext;
        private readonly ICosmosDbReadClient cosmosDbReadClient;
        private readonly ICosmosDbWriteClient cosmosDbWriteClient;
        private readonly IRuleEngine ruleEngine;

        public UserActivityService(
            IBlobStorageContext blobStorageContext,
            IEventHubContext eventhubContext,
            ICosmosDbReadClient cosmosDbReadClient,
            ICosmosDbWriteClient cosmosDbWriteClient,
            IRuleEngine ruleEngine)
        {
            this.blobStorageContext = blobStorageContext;
            this.eventhubContext = eventhubContext;
            this.cosmosDbReadClient = cosmosDbReadClient;
            this.cosmosDbWriteClient = cosmosDbWriteClient;
            this.ruleEngine = ruleEngine;
        }

        public async Task<IBaseResponse> ProcessAddActivityRequest(
            HttpRequest req,
            IAsyncCollector<string> outputEvents,
            string containerName,
            ILogger logger)
        {
            string userId = req.Query["userId"];

            if (string.IsNullOrWhiteSpace(userId))
            {
                return new IBaseResponse(ResponseCode.INVALID_USER_ID);
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var activityRequest = JsonConvertUtil.DeserializeActivityRequest(requestBody);

            if (activityRequest == null)
            {
                return new IBaseResponse(ResponseCode.INVALID_REQUEST_BODY);
            }

            var response = activityRequest.ValidateData();
            if (response != ResponseCode.SUCCESS)
            {
                return new IBaseResponse(response);
            }

            activityRequest.UserId = userId;
            response = await this.blobStorageContext.UploadAddActivityRequest(activityRequest, containerName, logger);
            if (response != ResponseCode.SUCCESS)
            {
                return new IBaseResponse(response);
            }

            return await this.eventhubContext.AddActivityMessage(activityRequest, outputEvents);
        }

        public async Task ProcessEvents(
            EventData[] events,
            ExecutionContext context,
            ILogger log)
        {
            var exceptions = new List<Exception>();

            foreach (EventData eventData in events)
            {
                try
                {
                    string messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                    var actityRequest = JsonConvertUtil.DeserializeActivityRequest(messageBody);

                    await this.ProcessActivityRequest(
                        actityRequest,
                        context,
                        log);

                    await Task.Yield();
                }
                catch (Exception e)
                {
                    // We need to keep processing the rest of the batch - capture this exception and continue.
                    exceptions.Add(e);
                }
            }

            // Once processing of the batch is complete, if any messages in the batch failed processing throw an exception so that there is a record of the failure.
            if (exceptions.Count > 1)
            {
                log.LogError($"Failed to proecess {exceptions.Count} events.");
                throw new AggregateException(exceptions);
            }

            if (exceptions.Count == 1)
            {
                log.LogError($"Failed to process one event.");
                throw exceptions.Single();
            }
        }

        private async Task ProcessActivityRequest(
            ActivityRequest actityRequest,
            ExecutionContext context,
            ILogger log)
        {
            var activity = actityRequest.ToReputationActivity();

            var userId = actityRequest.UserId;
            var userProfile = await CosmosDbReadUtility.GetUserReputation(this.cosmosDbReadClient, userId);
            userProfile ??= actityRequest.ToUserProfile();

            await this.ruleEngine.Process(
                userProfile: userProfile,
                activity: activity,
                logger: log);

            await this.cosmosDbWriteClient.ExecuteStoredProcedureAsync(
                context: context,
                partitionKey: "/userId",
                userProfileDoc: userProfile,
                activityDoc: activity);

            // For Leaderboard
            //await this.multiLeaderboardProcessor.ProcessMultiLeaderboard(userProfile, activity);
        }
    }
}
