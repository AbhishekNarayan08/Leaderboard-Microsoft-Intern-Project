// <copyright file="CosmosDbLeaderboardWriteClient.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace CosmosDB.Services.Data
{
    using System.Threading.Tasks;
    using Configs;
    using CosmosDB.Interfaces;
    using Microsoft.Azure.Cosmos;

    public class CosmosDbLeaderboardWriteClient : CosmosDbClient, ICosmosDbLeaderboardWriteClient
    {
        private readonly Container container;
        private readonly CosmosDbDatabaseName databaseName;
        private readonly CosmosDbCollectionName containerName;

        public CosmosDbLeaderboardWriteClient(ICosmosDbConfig cosmosDbConfig)
            : base(cosmosDbConfig)
        {
            this.databaseName = CosmosDbDatabaseName.Reputation;
            this.containerName = CosmosDbCollectionName.LeaderboardUser;
            this.container = this.Client.GetContainer(this.databaseName.ToString(), this.containerName.ToString());
        }

        public async Task UpsertDocumentAsync<T>(string partitionKeyPath, T document)
        {
            await this.container.UpsertItemAsync<T>(item: document);
        }
    }
}
