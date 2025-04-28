// <copyright file="CosmosDbWriteClient.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace CosmosDB.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Configs;
    using CosmosDB.Interfaces;
    using CosmosDB.Models;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Azure.Cosmos.Scripts;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;

    public class CosmosDbWriteClient : CosmosDbClient, ICosmosDbWriteClient
    {
        private CosmosDbDatabaseName DatabaseName;
        private CosmosDbCollectionName ContainerName;
        private readonly Container Container;
        private readonly string StoredProcedureId = "spUpsertUserProfileAndCreateActiviy";
        private readonly string BulkWriteStoreProcedureId = "spBulkWrite";
        private readonly int MaxInRequest = 10000;
        private readonly ILogger<ICosmosDbWriteClient> Logger;

        public CosmosDbWriteClient(ICosmosDbConfig cosmosDbConfig, ILogger<ICosmosDbWriteClient> logger)
            : base(cosmosDbConfig)
        {
            this.DatabaseName = CosmosDbDatabaseName.Reputation;
            this.ContainerName = CosmosDbCollectionName.Users;
            this.Container = this.Client.GetContainer(this.DatabaseName.ToString(), this.ContainerName.ToString());
            this.Logger = logger;
        }

        public async Task UpsertDocumentAsync<T>(
            string partitionKeyPath,
            T document)
        {
            try
            {

                var response = await this.Container.UpsertItemAsync<T>(
                    item: document);

                var requestChange = response.RequestCharge;

                this.Logger.LogInformation($"CosmosDB Upsert RUs    Cost = {requestChange} RUs");
            }
            catch (Exception ex)
            {
                this.Logger.LogInformation(ex.Message);
            }
        }

        public Task DeleteDocumentById<T>(string partitionKeyPath, string partitionKeyValue, string id)
        {
            throw new NotImplementedException();
        }

        public async Task ExecuteStoredProcedureAsync(
            ExecutionContext context,
            string partitionKey,
            dynamic userProfileDoc,
            dynamic activityDoc)
        {
            try
            {
                var storedProcedureResponse = await this.GetOrCreateStoredProcedure(
                    this.StoredProcedureId,
                    context);

                if (storedProcedureResponse.StatusCode == HttpStatusCode.OK
                    || storedProcedureResponse.StatusCode == HttpStatusCode.Created)
                {
                    var arguments = new dynamic[]
                    {
                    activityDoc,
                    userProfileDoc,
                    CosmosDocType.UserProfile.ToString(),
                    };

                    var result = await this.Container.Scripts.ExecuteStoredProcedureAsync<string>(
                        storedProcedureResponse.Resource.Id,
                        new Microsoft.Azure.Cosmos.PartitionKey(activityDoc.UserId),
                        arguments,
                        new StoredProcedureRequestOptions { EnableScriptLogging = true });

                    this.Logger.LogInformation("Result {0}", result.Resource);
                    this.Logger.LogInformation(result.ScriptLog);
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex.Message);
            }
        }

        public async Task<BulkOperationResponse<T>> ExecuteBatchAsync<T>(string partitionKey, List<dynamic> items)
        {
            var bulkTransaction = new BulkOperation<T>(this.Client, this.StoredProcedureId, this.Container);

            try
            {

                BulkOperationResponse<T> bulkOperationResponse = await bulkTransaction.ExecuteAsync(items, partitionKey);

                return bulkOperationResponse;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex.Message);
            }

            return null;
        }

        public async Task<bool> BulkWriteAsync(
            ExecutionContext context,
            string partitionKey,
            dynamic userProfileDoc,
            List<ReputationActivity> activityDocs)
        {
            try
            {
                var storedProcedureResponse = await this.GetOrCreateStoredProcedure(
                    this.BulkWriteStoreProcedureId,
                    context);

                if (storedProcedureResponse.StatusCode == HttpStatusCode.Created || storedProcedureResponse.StatusCode == HttpStatusCode.OK)
                {
                    var totalInserted = 0;
                    var totalDocumentCount = activityDocs.Count;
                    while (totalInserted < totalDocumentCount)
                    {
                        var activityDocSet = activityDocs.Take(this.MaxInRequest);

                        var result = await this.Container.Scripts.ExecuteStoredProcedureAsync<BulkWriteResponse>(
                            storedProcedureResponse.Resource.Id,
                            new Microsoft.Azure.Cosmos.PartitionKey(userProfileDoc.UserId),
                            new[] { activityDocSet, userProfileDoc, CosmosDocType.UserProfile.ToString() },
                            new StoredProcedureRequestOptions { EnableScriptLogging = true });

                        var insertedResponse = result.Resource;
                        totalInserted += insertedResponse.Count;
                        var remaining = totalDocumentCount - totalInserted;
                        this.Logger.LogDebug($"Message  {insertedResponse.ErrorMessage}");
                        this.Logger.LogDebug($"Inserted or Updated {insertedResponse.Count} documents ({totalInserted} total, {remaining} remaining)");
                        activityDocs = activityDocs.GetRange(insertedResponse.Count, activityDocs.Count - insertedResponse.Count);
                    }
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex.Message);
                throw;
            }

            return true;
        }

        public async Task<StoredProcedureResponse> GetOrCreateStoredProcedure(
            string storedProcedureId,
            ExecutionContext context)
        {
            var spFIlePath = Path.Combine(context.FunctionAppDirectory, $@"Services\Server\{storedProcedureId}.js");
            var sprocBody = File.ReadAllText(spFIlePath);
            var sprocInfo = new StoredProcedureProperties
            {
                Id = storedProcedureId,
                Body = sprocBody,
            };

            var localVersion = this.GetStoredProcedureVersion(sprocInfo);

            StoredProcedureResponse sprocResource;
            try
            {
                sprocResource = await this.Container.Scripts.ReadStoredProcedureAsync(sprocInfo.Id);
                var remoteVersion = this.GetStoredProcedureVersion(sprocResource.Resource);

                if (remoteVersion == null || localVersion == null)
                {
                    return null;
                }

                bool isNewer = remoteVersion < localVersion;
                this.Logger.LogDebug($"Found sproc '{sprocInfo.Id}' at version {remoteVersion} (local version is {localVersion})");

                if (!isNewer)
                {
                    return sprocResource;
                }
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                sprocResource = null;
            }
            catch (Exception ex)
            {
                this.Logger.LogDebug($"DocumentDbStoredProcedureManager: Error getting stored procedure. Exception: {ex}");
                throw;
            }

            try
            {
                if (sprocResource != null)
                {
                    sprocResource = await this.Container.Scripts.ReplaceStoredProcedureAsync(sprocInfo);
                    this.Logger.LogDebug($"(Re-created sproc '{sprocInfo.Id}");
                }
                else
                {
                    sprocResource = await this.Container.Scripts.CreateStoredProcedureAsync(sprocInfo);
                    this.Logger.LogDebug($"(Created sproc '{sprocInfo.Id}");
                }
            }
            catch (Exception dex)
            {
                this.Logger.LogInformation($"Error creating/replacing sproc '{sprocInfo.Id}'. Error: {dex.Message}");
                throw;
            }

            return sprocResource;
        }

        public int? GetStoredProcedureVersion(StoredProcedureProperties sprocResource)
        {
            var body = sprocResource.Body;
            var idx = body.IndexOf("]");
            if (idx < 0)
            {
                return null;
            }

            var maybeVersionString = body.Substring(0, idx);
            var prefix = "//[version=";
            if (!maybeVersionString.StartsWith(prefix))
            {
                return null;
            }

            var maybeVersionNumber = maybeVersionString.Substring(prefix.Length);
            int version;
            if (!int.TryParse(maybeVersionNumber, out version))
            {
                return null;
            }

            return version;
        }
    }
}
