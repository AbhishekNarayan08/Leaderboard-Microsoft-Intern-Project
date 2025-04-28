// <copyright file="ActivityFeedServiceFactory.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ReadApis.Factories
{
    using System;
    using GetActivityFeedProcessor;
    using GetReputationProcessor;

    public class ActivityFeedServiceFactory : IActivityFeedServiceFactory
    {
        private readonly IServiceProvider serviceProvider;

        public ActivityFeedServiceFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IGetActivityFeedService GetActivityFeedService<T>()
            where T : IGetActivityFeedService
        {
            return (IGetActivityFeedService)this.serviceProvider.GetService(typeof(T));
        }
    }
}
