// <copyright file="CosmosDbUserLeaderboardWriteClient.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace CosmosDB.Services.Data
{
    using System.Threading.Tasks;
    using Configs;
    using CosmosDB.Interfaces;
    using Microsoft.Azure.Cosmos;

    public class CosmosDbUserLeaderboardWriteClient : CosmosDbClient, ICosmosDbUserLeaderboardWriteClient
    {
        private readonly Container container;
        private readonly CosmosDbDatabaseName databaseName;
        private readonly CosmosDbCollectionName containerName;

        public CosmosDbUserLeaderboardWriteClient(ICosmosDbConfig cosmosDbConfig)
            : base(cosmosDbConfig)
        {
            this.databaseName = CosmosDbDatabaseName.Reputation;
            this.containerName = CosmosDbCollectionName.UserLeaderboards;
            this.container = this.Client.GetContainer(this.databaseName.ToString(), this.containerName.ToString());
        }

        public async Task UpsertDocumentAsync<T>(string partitionKeyPath, T document)
        {
            await this.container.UpsertItemAsync<T>(item: document);
        }
    }
}
