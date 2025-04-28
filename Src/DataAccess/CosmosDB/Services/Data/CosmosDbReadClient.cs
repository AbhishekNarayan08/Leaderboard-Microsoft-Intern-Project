// <copyright file="CosmosDbReadClient.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace CosmosDB.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Configs;
    using CosmosDB.Interfaces;
    using Microsoft.Azure.Cosmos;

    public class CosmosDbReadClient : CosmosDbClient, ICosmosDbReadClient
    {
        private CosmosDbDatabaseName DatabaseName;
        private CosmosDbCollectionName ContainerName;
        private int ResultPerPage = 25;

        public CosmosDbReadClient(
           ICosmosDbConfig cosmosDbConfig)
           : base(cosmosDbConfig)
        {
            this.DatabaseName = CosmosDbDatabaseName.Reputation;
            this.ContainerName = CosmosDbCollectionName.Users;
        }

        public async Task<FeedResponse<T>> QuerySinglePageAsync<T>(
           QueryDefinitionWithParams query,
           string partitionKey)
        {
            return await this.QuerySinglePageAsyncInternal<T>(
                query,
                partitionKeyPath: partitionKey);
        }

        public async IAsyncEnumerable<T> QueryAsync<T>(
            QueryDefinitionWithParams query,
            string partitionKey)
        {
           await foreach (var item in this.QueryAsyncInternal<T>(
                query,
                partitionKey: partitionKey))
            {
                yield return item;
            }
        }

        private async IAsyncEnumerable<T> QueryAsyncInternal<T>(
            QueryDefinitionWithParams query,
            string partitionKey)
        {

            var container = this.Client.GetContainer(this.DatabaseName.ToString(), this.ContainerName.ToString());

            var iterator = container.GetItemQueryIterator<T>(
                queryDefinition: query,
                requestOptions: new QueryRequestOptions()
                {
                    MaxItemCount = this.ResultPerPage,
                    PartitionKey = new PartitionKey(partitionKey),
                });

            var page = 0;
            var totalRequestCharge = 0d;

            while (iterator.HasMoreResults)
            {
                page++;

                var response = await iterator.ReadNextAsync();

                var requestChange = response.RequestCharge;
                totalRequestCharge += requestChange;

                Console.WriteLine("Received {0} results with a RU cost of {1} for page {2}", response.Count, requestChange, page, query);

                foreach (var item in response)
                {
                    yield return item;
                }
            }
        }

        private async Task<FeedResponse<T>> QuerySinglePageAsyncInternal<T>(
            QueryDefinitionWithParams query,
            string partitionKeyPath)
        {

            var container = this.Client.GetContainer(this.DatabaseName.ToString(), this.ContainerName.ToString());

            var setIterator = container.GetItemQueryIterator<T>(
                queryDefinition: query,
                requestOptions: new QueryRequestOptions()
                {
                    MaxItemCount = this.ResultPerPage,
                });

            var response = await setIterator.ReadNextAsync();

            var requestChange = response.RequestCharge;

            return response;
        }
    }
}
