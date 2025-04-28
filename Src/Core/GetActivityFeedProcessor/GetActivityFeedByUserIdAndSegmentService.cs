// <copyright file="GetActivityFeedByUserIdAndSegmentService.cs" company="Microsoft">
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

    public class GetActivityFeedByUserIdAndSegmentService : IGetActivityFeedService
    {
        private readonly ICosmosDbReadClient cosmosDbReadClient;

        public GetActivityFeedByUserIdAndSegmentService(ICosmosDbReadClient cosmosDbReadClient)
        {
            this.cosmosDbReadClient = cosmosDbReadClient;
        }

        public async Task<IBaseResponse> GetActivityFeed(HttpRequest req, ILogger logger)
        {
            var userId = ApiHelper.GetUserId(req);
            var segmentType = ApiHelper.GetSegmentType(req);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return new ActivityFeedByUserId(ResponseCode.INVALID_USER_ID);
            }

            if (segmentType == SegmentType.Unknown)
            {
                return new ActivityFeedByUserId(ResponseCode.INVALID_SEGMENT_ID);
            }

            var cosmosDbResponse = await CosmosDbReadUtility.GetActivitiyFeed(this.cosmosDbReadClient, userId);

            var activityFeedByUserIdAndSegment = CosmosDbResponseHelper.ReputationActivityListToActivityFeedByUserIdAndSegment(cosmosDbResponse, segmentType);

            return activityFeedByUserIdAndSegment;
        }
    }
}
