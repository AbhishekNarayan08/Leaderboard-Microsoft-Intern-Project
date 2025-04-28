// <copyright file="BulkOperation.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace CosmosDB.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CosmosDB.Interfaces;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Azure.Cosmos.Scripts;

    public class BulkOperation<T> : IBulkOperation<T>
    {
        private string StoredProcedureId;
        private readonly List<Task<OperationResponse<T>>> Tasks;
        private IOperationResponse<T> TransactionResponse;
        private CosmosClient Client;
        private Container Container;

        public BulkOperation(
            CosmosClient client,
            string storedProcedureId,
            Container container)
        {
            this.StoredProcedureId = storedProcedureId;
            this.Client = client;
            this.Container = container;

            this.Tasks = new List<Task<OperationResponse<T>>>();
            this.TransactionResponse = new OperationResponse<T>();
        }

        public async Task<BulkOperationResponse<T>> ExecuteAsync(List<dynamic> items, string partitionKey)
        {
            foreach (var item in items)
            {
                this.Tasks.Add(this.TransactionResponse.CaptureResponse(this.Container.Scripts.ExecuteStoredProcedureAsync<string>(this.StoredProcedureId,
                    new PartitionKey(partitionKey),
                    new[] { item },
                    new StoredProcedureRequestOptions { EnableScriptLogging = true }), item));
            }

            await Task.WhenAll(this.Tasks);

            return new BulkOperationResponse<T>()
            {
                TotalRequestUnitsConsumed = this.Tasks.Sum(task => task.Result.RequestUnitsConsumed),
                SuccessfulDocuments = this.Tasks.Count(task => task.Result.IsSuccessful),
                Failures = this.Tasks.Where(task => !task.Result.IsSuccessful).Select(task => (task.Result.Item, task.Result.CosmosException)).ToList()
            };
        }
    }
}
