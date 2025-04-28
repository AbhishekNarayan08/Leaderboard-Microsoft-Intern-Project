// <copyright file="ReputationServiceFactory.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ReadApis.Factories
{
    using System;

    using GetReputationProcessor;

    public class ReputationServiceFactory : IReputationServiceFactory
    {
        private readonly IServiceProvider serviceProvider;

        public ReputationServiceFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IGetReputationService GetReputationService<T>()
            where T : IGetReputationService
        {
            return (IGetReputationService)this.serviceProvider.GetService(typeof(T));
        }
    }
}
