// <copyright file="IReputationServiceFactory.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ReadApis.Factories
{
    using GetReputationProcessor;

    public interface IReputationServiceFactory
    {
        public IGetReputationService GetReputationService<T>()
            where T : IGetReputationService;
    }
}
