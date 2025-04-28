// <copyright file="CosmosDbLeaderboardReadClient.cs" company="Microsoft">
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

    public class CosmosDbLeaderboardReadClient : CosmosDbClient, ICosmosDbLeaderboardReadClient
    {
        private readonly CosmosDbDatabaseName databaseName;
        private readonly CosmosDbCollectionName containerName;
        private readonly int resultPerPage = 25;

        public CosmosDbLeaderboardReadClient(
           ICosmosDbConfig cosmosDbConfig)
           : base(cosmosDbConfig)
        {
            this.databaseName = CosmosDbDatabaseName.Reputation;
            this.containerName = CosmosDbCollectionName.Leaderboards;
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
            var container = this.Client.GetContainer(this.databaseName.ToString(), this.containerName.ToString());

            var iterator = container.GetItemQueryIterator<T>(
                queryDefinition: query,
                requestOptions: new QueryRequestOptions()
                {
                    MaxItemCount = this.resultPerPage,
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
            var container = this.Client.GetContainer(this.databaseName.ToString(), this.containerName.ToString());

            var setIterator = container.GetItemQueryIterator<T>(
                queryDefinition: query,
                requestOptions: new QueryRequestOptions()
                {
                    MaxItemCount = this.resultPerPage,
                });

            var response = await setIterator.ReadNextAsync();

            var requestChange = response.RequestCharge;

            return response;
        }
    }
}
