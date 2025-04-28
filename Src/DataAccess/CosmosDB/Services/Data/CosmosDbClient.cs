// <copyright file="CosmosDbClient.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace CosmosDB.Services.Data
{
    using System;
    using Configs;
    using Microsoft.Azure.Cosmos;
    using Newtonsoft.Json;

    public class CosmosDbClient
    {
        protected CosmosClient Client { get; private set; }

        public CosmosDbClient(ICosmosDbConfig cosmosDbConfig)
        {
            this.Client = new CosmosClient(
                accountEndpoint: cosmosDbConfig.EndPointUrl,
                authKeyOrResourceToken: cosmosDbConfig.PrimaryKey,
                clientOptions: new CosmosClientOptions
                {
                    ConnectionMode = this.GetConnectionMode(),
                    RequestTimeout = new TimeSpan(hours: 0, minutes: 0, seconds: 4),
                    MaxRetryAttemptsOnRateLimitedRequests = 10,
                    MaxRetryWaitTimeOnRateLimitedRequests = new TimeSpan(hours: 0, minutes: 0, seconds: 30)
                });
        }

        private ConnectionMode GetConnectionMode() => ConnectionMode.Gateway;

        private JsonSerializerSettings GetJsonSerializerSettings() => new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            DateParseHandling = DateParseHandling.None,
        };
    }
}
