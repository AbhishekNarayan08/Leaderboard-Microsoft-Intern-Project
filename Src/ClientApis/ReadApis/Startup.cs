// <copyright file="Startup.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using Common.Helpers;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using ReadApis.Factories;
using GetReputationProcessor;
using GetActivityFeedProcessor;
using CosmosDB.Interfaces;
using CosmosDB.Services.Data;

[assembly: FunctionsStartup(typeof(ReadApis.Startup))]

namespace ReadApis
{
    public class Startup : CommonStartup
    {
        public override void AddAdditionalServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(ICosmosDbReadClient), typeof(CosmosDbReadClient));

            services.AddSingleton(typeof(IReputationServiceFactory), typeof(ReputationServiceFactory));
            services.AddSingleton(typeof(IActivityFeedServiceFactory), typeof(ActivityFeedServiceFactory));

            services.AddSingleton<GetReputationByUserIdService>();
            services.AddSingleton<GetReputationByUserIdBatchService>();
            services.AddSingleton<GetReputationByUserIdAndSegmentService>();
            services.AddSingleton<GetReputationByUserIdAndSegmentBatchService>();

            services.AddSingleton<GetActivityFeedByUserIdService>();
            services.AddSingleton<GetActivityFeedByUserIdAndSegmentService>();
        }
    }
}
