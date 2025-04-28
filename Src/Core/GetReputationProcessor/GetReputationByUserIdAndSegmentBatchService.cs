// <copyright file="GetReputationByUserIdAndSegmentBatchService.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace GetReputationProcessor
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

    public class GetReputationByUserIdAndSegmentBatchService : IGetReputationService
    {
        private readonly ICosmosDbReadClient cosmosDbReadClient;

        public GetReputationByUserIdAndSegmentBatchService(ICosmosDbReadClient cosmosDbReadClient)
        {
            this.cosmosDbReadClient = cosmosDbReadClient;
        }

        public async Task<IBaseResponse> GetReputation(HttpRequest req, ILogger logger)
        {
            var userIds = ApiHelper.GetUserIds(req);

            var segmentType = ApiHelper.GetSegmentType(req);

            if (segmentType == SegmentType.Unknown)
            {
                return new ReputationByUserIdAndSegmentBatch(ResponseCode.INVALID_SEGMENT_ID);
            }

            var reputationByUserIdAndSegmentList = new List<ReputationByUserIdAndSegment>();

            foreach (var userId in userIds)
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    reputationByUserIdAndSegmentList.Add(new ReputationByUserIdAndSegment(ResponseCode.INVALID_USER_ID));
                    continue;
                }

                var cosmosDbResponse = await CosmosDbReadUtility.GetUserReputation(this.cosmosDbReadClient, userId);

                if (cosmosDbResponse == null)
                {
                    reputationByUserIdAndSegmentList.Add(new ReputationByUserIdAndSegment(ResponseCode.INVALID_USER_ID));
                    continue;
                }

                var reputationByUserIdAndSegment = CosmosDbResponseHelper.UserProfileToReputationByUserIdAndSegment(cosmosDbResponse, segmentType);

                reputationByUserIdAndSegmentList.Add(reputationByUserIdAndSegment);
            }

            if (reputationByUserIdAndSegmentList.Count == 0)
            {
                return new ReputationByUserIdAndSegmentBatch(ResponseCode.INVALID_USER_ID);
            }

            return new ReputationByUserIdAndSegmentBatch(ResponseCode.SUCCESS, reputationByUserIdAndSegmentList);
        }
    }
}
