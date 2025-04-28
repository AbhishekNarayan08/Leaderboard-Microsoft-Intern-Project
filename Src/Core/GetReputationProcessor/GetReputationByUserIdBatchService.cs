// <copyright file="GetReputationByUserIdBatchService.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace GetReputationProcessor
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Helpers;
    using Common.Models.Responses;
    using Common.Utils;
    using CosmosDB.Interfaces;
    using CosmosDB.Util;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    public class GetReputationByUserIdBatchService : IGetReputationService
    {
        private readonly ICosmosDbReadClient cosmosDbReadClient;

        public GetReputationByUserIdBatchService(ICosmosDbReadClient cosmosDbReadClient)
        {
            this.cosmosDbReadClient = cosmosDbReadClient;
        }

        public async Task<IBaseResponse> GetReputation(HttpRequest req, ILogger logger)
        {
            var userIds = ApiHelper.GetUserIds(req);

            var reputationByUserIdList = new List<ReputationByUserId>();

            foreach (var userId in userIds)
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    reputationByUserIdList.Add(new ReputationByUserId(ResponseCode.INVALID_USER_ID));
                    continue;
                }

                var cosmosDbResponse = await CosmosDbReadUtility.GetUserReputation(this.cosmosDbReadClient, userId);

                if (cosmosDbResponse == null)
                {
                    reputationByUserIdList.Add(new ReputationByUserId(ResponseCode.INVALID_USER_ID));
                    continue;
                }

                var reputationByUserId = CosmosDbResponseHelper.UserProfileToReputationByUserId(cosmosDbResponse);
                reputationByUserIdList.Add(reputationByUserId);
            }

            if (reputationByUserIdList.Count == 0)
            {
                return new ReputationByUserIdBatch(ResponseCode.INVALID_USER_ID);
            }

            return new ReputationByUserIdBatch(ResponseCode.SUCCESS, reputationByUserIdList);
        }
    }
}
