// <copyright file="IActivityFeedServiceFactory.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ReadApis.Factories
{
    using GetActivityFeedProcessor;

    public interface IActivityFeedServiceFactory
    {
        public IGetActivityFeedService GetActivityFeedService<T>()
            where T : IGetActivityFeedService;
    }
}
