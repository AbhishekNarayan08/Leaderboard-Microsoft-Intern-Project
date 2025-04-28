// <copyright file="GetReputationByUserIdService.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace GetReputationProcessor
{
    using System.Threading.Tasks;
    using Common.Helpers;
    using Common.Models.Responses;
    using Common.Utils;
    using CosmosDB.Interfaces;
    using CosmosDB.Util;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    public class GetReputationByUserIdService : IGetReputationService
    {
        private readonly ICosmosDbReadClient cosmosDbReadClient;

        public GetReputationByUserIdService(ICosmosDbReadClient cosmosDbReadClient)
        {
            this.cosmosDbReadClient = cosmosDbReadClient;
        }

        public async Task<IBaseResponse> GetReputation(HttpRequest req, ILogger logger)
        {
            string userId = ApiHelper.GetUserId(req);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return new ReputationByUserId(ResponseCode.INVALID_USER_ID);
            }

            var cosmosDbResponse = await CosmosDbReadUtility.GetUserReputation(this.cosmosDbReadClient, userId);

            if (cosmosDbResponse == null)
            {
                return new ReputationByUserId(ResponseCode.INVALID_USER_ID);
            }

            var reputationByUserId = CosmosDbResponseHelper.UserProfileToReputationByUserId(cosmosDbResponse);

            return reputationByUserId;
        }
    }
}
