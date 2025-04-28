// <copyright file="IGetReputationService.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace GetReputationProcessor
{
    using System.Threading.Tasks;

    using Common.Models.Responses;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    public interface IGetReputationService
    {
        public Task<IBaseResponse> GetReputation(HttpRequest req, ILogger logger);
    }
}
