// <copyright file="GetReputationByUserIdAndSegmentService.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace GetReputationProcessor
{
    using System.Threading.Tasks;
    using Common.Helpers;
    using Common.Models.Enums;
    using Common.Models.Responses;
    using Common.Utils;
    using CosmosDB.Interfaces;
    using CosmosDB.Util;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    public class GetReputationByUserIdAndSegmentService : IGetReputationService
    {
        private readonly ICosmosDbReadClient cosmosDbReadClient;

        public GetReputationByUserIdAndSegmentService(ICosmosDbReadClient cosmosDbReadClient)
        {
            this.cosmosDbReadClient = cosmosDbReadClient;
        }

        public async Task<IBaseResponse> GetReputation(HttpRequest req, ILogger logger)
        {
            var userId = ApiHelper.GetUserId(req);

            var segmentType = ApiHelper.GetSegmentType(req);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return new ReputationByUserIdAndSegment(ResponseCode.INVALID_USER_ID);
            }

            if (segmentType == SegmentType.Unknown)
            {
                return new ReputationByUserIdAndSegment(ResponseCode.INVALID_SEGMENT_ID);
            }

            var cosmosDbResponse = await CosmosDbReadUtility.GetUserReputation(this.cosmosDbReadClient, userId);

            if (cosmosDbResponse == null)
            {
                return new ReputationByUserIdAndSegment(ResponseCode.INVALID_USER_ID);
            }

            var reputationByUserIdAndSegment = CosmosDbResponseHelper.UserProfileToReputationByUserIdAndSegment(cosmosDbResponse, segmentType);

            return reputationByUserIdAndSegment;
        }
    }
}
