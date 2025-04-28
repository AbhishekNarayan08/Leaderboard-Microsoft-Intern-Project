using AzureStorage;
using BulkActivityProcessor;
using Common.Helpers;
using CosmosDB.Interfaces;
using CosmosDB.Services.Data;
using EventHub;
using LeaderboardProcessor;
using Microsoft.Azure.Amqp.Serialization;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using RedisCache.Interfaces;
using RedisCache.Services;
using RuleEngine.Core;
using RuleEngine.Rules;
using System;
using UserActivityProcessor;

[assembly: FunctionsStartup(typeof(LeaderboardAPIs.Startup))]

namespace LeaderboardAPIs
{
    public class Startup : CommonStartup
    {
        // Function Input bindings
        public override void AddAdditionalServices(IServiceCollection services)
        {
            // Cosmos DB Clients
            services.AddSingleton(typeof(ICosmosDbWriteClient), typeof(CosmosDbWriteClient));
            services.AddSingleton(typeof(ICosmosDbReadClient), typeof(CosmosDbReadClient));

            // Cosmos DB Leaderboard Client
            services.AddSingleton(typeof(ICosmosDbUserLeaderboardReadClient), typeof(CosmosDbUserLeaderboardReadClient));
            services.AddSingleton(typeof(ICosmosDbUserLeaderboardWriteClient), typeof(CosmosDbUserLeaderboardWriteClient));

            // Leaderboard
            services.AddSingleton(typeof(IMultiLeaderboardProcessor), typeof(MultiLeaderboardProcessor));

            // Redis Clients
            services.AddSingleton(typeof(IRedisWriteClient), typeof(RedisWriteClient));
            services.AddSingleton(typeof(IRedisReadClient), typeof(RedisReadClient));
        }
    }
}
