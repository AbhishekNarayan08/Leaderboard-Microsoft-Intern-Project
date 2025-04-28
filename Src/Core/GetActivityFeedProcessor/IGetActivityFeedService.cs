// <copyright file="IGetActivityFeedService.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace GetActivityFeedProcessor
{
    using System.Threading.Tasks;

    using Common.Models.Responses;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    public interface IGetActivityFeedService
    {
        public Task<IBaseResponse> GetActivityFeed(HttpRequest req, ILogger logger);
    }
}
