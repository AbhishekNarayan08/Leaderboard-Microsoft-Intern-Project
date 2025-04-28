// <copyright file="CommonStartup.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Common.Helpers
{
    using Configs;
    using Microsoft.Azure.Functions.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection;

    public class CommonStartup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder) => this.AddServices(builder.Services);

        /// <summary>
        /// Method to register services both at common code and sports specific overrides.
        /// </summary>
        /// <param name="services">ServiceCollection object.</param>
        public void AddServices(IServiceCollection services)
        {
            //// Direct Dependency Injection of Common Services including data ingestion modules and
            //// all common utility services.
            this.RegisterCommonDependencies(services);

            //// This line always has to stay as last section of this method
            //// block. Rearranging this sequence is going to break sports specific DI where we want
            //// Sports based registratino to preceed common default objects.
            this.AddAdditionalServices(services);
        }

        public virtual void AddAdditionalServices(IServiceCollection services)
        {
        }

        private void RegisterCommonDependencies(IServiceCollection services)
        {
            services.AddLogging();
            services.AddSingleton(typeof(IAzureStorageConfig), typeof(AzureStorageConfig));
            services.AddSingleton(typeof(IEventHubConfig), typeof(EventHubConfig));
            services.AddSingleton(typeof(ICosmosDbConfig), typeof(CosmosDbConfig));
        }
    }
}
