// <copyright file="Startup.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using AzureStorage;
using BulkActivityProcessor;
using Common.Helpers;
using CosmosDB.Interfaces;
using CosmosDB.Services.Data;
using EventHub;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using RuleEngine.Core;
using RuleEngine.Rules;
using UserActivityProcessor;

[assembly: FunctionsStartup(typeof(WriteApis.Startup))]

namespace WriteApis
{
    public class Startup : CommonStartup
    {
        public override void AddAdditionalServices(IServiceCollection services)
        {
            // Function Input bindings
            services.AddSingleton(typeof(IBlobStorageContext), typeof(BlobStorageContext));
            services.AddSingleton(typeof(IEventHubContext), typeof(EventHubContext));

            // Cosmos DB Clients
            services.AddSingleton(typeof(ICosmosDbWriteClient), typeof(CosmosDbWriteClient));
            services.AddSingleton(typeof(ICosmosDbReadClient), typeof(CosmosDbReadClient));

            // Cosmos DB Leaderboard Client
            services.AddSingleton(typeof(ICosmosDbLeaderboardReadClient), typeof(CosmosDbLeaderboardReadClient));

            // Reputation Core Services
            services.AddSingleton(typeof(IUserActivityService), typeof(UserActivityService));
            services.AddSingleton(typeof(IBulkActivityService), typeof(BulkActivityService));

            // Rule Engine
            services.AddSingleton(typeof(IRuleRepository), typeof(RuleRepository));
            services.AddSingleton(typeof(IRuleEngine), typeof(RuleEngine.Core.RuleEngine));
        }
    }
}
