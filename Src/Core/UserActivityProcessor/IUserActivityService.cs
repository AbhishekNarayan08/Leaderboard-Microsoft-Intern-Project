// <copyright file="IUserActivityService.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace UserActivityProcessor
{
    using System.Threading.Tasks;
    using Common.Models.Responses;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.EventHubs;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;

    public interface IUserActivityService
    {
        public Task<IBaseResponse> ProcessAddActivityRequest(
            HttpRequest req,
            IAsyncCollector<string> outputEvents,
            string containerName,
            ILogger logger);

        public Task ProcessEvents(
            EventData[] events,
            ExecutionContext context,
            ILogger logger);
    }
}
