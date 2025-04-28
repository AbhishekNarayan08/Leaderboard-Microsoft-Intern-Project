// <copyright file="GetActivityFeedByUserIdService.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace GetActivityFeedProcessor
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Helpers;
    using Common.Models.Enums;
    using Common.Models.Responses;
    using Common.Utils;
    using CosmosDB.Interfaces;
    using CosmosDB.Util;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    public class GetActivityFeedByUserIdService : IGetActivityFeedService
    {
        private readonly ICosmosDbReadClient cosmosDbReadClient;

        public GetActivityFeedByUserIdService(ICosmosDbReadClient cosmosDbReadClient)
        {
            this.cosmosDbReadClient = cosmosDbReadClient;
        }

        public async Task<IBaseResponse> GetActivityFeed(HttpRequest req, ILogger logger)
        {
            var userId = ApiHelper.GetUserId(req);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return new ActivityFeedByUserId(ResponseCode.INVALID_USER_ID);
            }

            var cosmosDbResponse = await CosmosDbReadUtility.GetActivitiyFeed(this.cosmosDbReadClient, userId);

            var activityFeedByUserId = CosmosDbResponseHelper.ReputationActivityListToActivityFeedByUserId(cosmosDbResponse);

            return activityFeedByUserId;
        }
    }
}
