// <copyright file="ICosmosDbWriteClient.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace CosmosDB.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CosmosDB.Models;
    using Microsoft.Azure.Cosmos.Scripts;
    using Microsoft.Azure.WebJobs;

    public interface ICosmosDbWriteClient
    {
        Task UpsertDocumentAsync<T>(
             string partitionKeyPath,
             T document);

        Task DeleteDocumentById<T>(
            string partitionKeyPath,
            string partitionKeyValue,
            string id);

        Task<StoredProcedureResponse> GetOrCreateStoredProcedure(
            string storedProcedureId,
            ExecutionContext context);

        Task ExecuteStoredProcedureAsync(
            ExecutionContext context,
            string partitionKey,
            dynamic userProfileDoc,
            dynamic activityDoc);

        Task<bool> BulkWriteAsync(
            ExecutionContext context,
            string partitionKey,
            dynamic userProfileDoc,
            List<ReputationActivity> activityDocs);
    }
}
