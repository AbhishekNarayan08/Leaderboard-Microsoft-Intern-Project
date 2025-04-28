// <copyright file="CosmosDbReadUtility.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace CosmosDB.Util
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Common.Helpers;
    using Common.Models.Constants;
    using Common.Utils;
    using CosmosDB.Interfaces;
    using CosmosDB.Models;
    using CosmosDB.Services.Data;
    using Microsoft.Azure.Cosmos;

    public static class CosmosDbReadUtility
    {
        public static async Task<UserProfile> GetUserReputation(ICosmosDbReadClient client, string userId)
        {
            userId = EncodeDecodeIdHelper.Encode(userId);
            var response = await client.QuerySinglePageAsync<UserProfile>(
                partitionKey: CosmosDbConstants.UserIdPartition,
                query: new QueryDefinitionWithParams($@"SELECT *
                    FROM Users users
                    WHERE users.userId = @userId AND users.docType = @docType").
                    WithParameter("@userId", userId).
                    WithParameter("@docType", EnumParserUtil.GetString(CosmosDocType.UserProfile)));

            bool isValid = IsValidResponse(response);

            if (isValid)
            {
                return response.Resource.FirstOrDefault();
            }

            return null;
        }

        public static async Task<List<UserProfile>> GetUserReputationBatch(ICosmosDbReadClient client, List<string> userIds)
        {
            var responses = new List<UserProfile>();

            foreach (var userId in userIds)
            {
                var encodedUserId = EncodeDecodeIdHelper.Encode(userId);
                var response = await client.QuerySinglePageAsync<UserProfile>(
                partitionKey: CosmosDbConstants.UserIdPartition,
                query: new QueryDefinitionWithParams($@"SELECT *
                    FROM Users users
                    WHERE users.userId = @userId AND users.docType = @docType").
                    WithParameter("@userId", encodedUserId).
                    WithParameter("@docType", EnumParserUtil.GetString(CosmosDocType.UserProfile)));

                if (IsValidResponse(response))
                {
                    responses.Add(response.Resource.FirstOrDefault());
                }
            }

            return responses;
        }

        public static async Task<List<ReputationActivity>> GetActivitiyFeed(ICosmosDbReadClient client, string userId)
        {
            var activities = new List<ReputationActivity>();
            userId = EncodeDecodeIdHelper.Encode(userId);
            var responses = client.QueryAsync<ReputationActivity>(
                partitionKey: CosmosDbConstants.UserIdPartition,
                query: new QueryDefinitionWithParams($@"SELECT *
                    FROM Users users
                    WHERE users.userId = @userId AND users.docType = @docType").
                    WithParameter("@userId", userId).
                    WithParameter("@docType", EnumParserUtil.GetString(CosmosDocType.Activity)));

            await foreach (var response in responses)
            {
                if (!IsValidResponse(response))
                {
                    continue;
                }

                activities.Add(response);
            }

            return activities;
        }

        private static bool IsValidResponse(FeedResponse<UserProfile> response)
        {
            if (response == null)
            {
                return false;
            }

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return false;
            }

            if (response.Resource == null || !response.Resource.Any())
            {
                return false;
            }

            var first = response.Resource.FirstOrDefault();

            if (first?.ReputationBySegment == null)
            {
                return false;
            }

            return true;
        }

        private static bool IsValidResponse(ReputationActivity response)
        {
            if (response == null)
            {
                return false;
            }

            return true;
        }
    }
}
