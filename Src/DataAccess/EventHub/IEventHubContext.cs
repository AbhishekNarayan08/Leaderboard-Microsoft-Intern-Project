// <copyright file="IEventHubContext.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace EventHub
{
    using System.Threading.Tasks;
    using Common.Models.Requests;
    using Common.Models.Responses;
    using Microsoft.Azure.WebJobs;

    public interface IEventHubContext
    {
        public Task<IBaseResponse> AddActivityMessage(ActivityRequest activityRequest, IAsyncCollector<string> outputEvents);
    }
}
