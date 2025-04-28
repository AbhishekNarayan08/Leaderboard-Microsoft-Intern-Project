using CosmosDB.Interfaces;
using CosmosDB.Models;
using CosmosDB.Services.Data;
using Microsoft.Azure.Cosmos;
using RedisCache.Interfaces;
using System.Runtime.CompilerServices;

namespace LeaderboardProcessor
{
    public class ColdStartProcessor
    {
        ICosmosDbReadClient cosmosDbReadClient;
        ICosmosDbUserLeaderboardWriteClient userLeaderboardWriteClient;
        ICosmosDbLeaderboardReadClient leaderboardReadClient;
        ICosmosDbLeaderboardWriteClient leaderboardWriteClient;
        IRedisReadClient redisReadClient;
        IRedisWriteClient redisWriteClient;

        public ColdStartProcessor(ICosmosDbReadClient cosmosDbReadClient, 
            ICosmosDbUserLeaderboardWriteClient userLeaderboardWriteClient, 
            IRedisReadClient redisReadClient, IRedisWriteClient redisWriteClient, 
            ICosmosDbLeaderboardReadClient leaderboardReadClient, 
            ICosmosDbLeaderboardWriteClient leaderboardWriteClient)
        {
            this.cosmosDbReadClient = cosmosDbReadClient;
            this.userLeaderboardWriteClient = userLeaderboardWriteClient;
            this.redisReadClient = redisReadClient;
            this.redisWriteClient = redisWriteClient;
            this.leaderboardReadClient = leaderboardReadClient;
            this.leaderboardWriteClient = leaderboardWriteClient;
        }

        public async Task CosmosColdStart()
        {
            var query = new QueryDefinitionWithParams("SELECT * FROM Users");

            var responses = cosmosDbReadClient.QueryAsync<UserProfile>(query: query, partitionKey: "/UserId");

            await foreach (var response in responses)
            {
                var dict = response.ReputationBySegment;
                UserLeaderboard userLeaderboard = new()
                {
                    UserId = response.UserId,
                };

                foreach (var item in dict)
                {
                    userLeaderboard.Leaderboards.Add(item.Key, item.Value.PointValue);

                    var paramQuery = new QueryDefinitionWithParams($@"SELECT *
                        FROM Leaderboards entry
                        WHERE entry.LeaderboardName = @leaderboardName").
                    WithParameter("@leaderboardName", item.Key);

                    var result = leaderboardReadClient.QuerySinglePageAsync<Leaderboard>(query: paramQuery, partitionKey: "/LeaderboardName");

                    if(result == null)
                    {
                        Leaderboard leaderboard = new Leaderboard();
                        leaderboard.LeaderboardName = item.Key;
                        leaderboard.Segment = item.Key;
                        await leaderboardWriteClient.UpsertDocumentAsync<Leaderboard>("/LeaderboardName", leaderboard);
                    }
                }

                await userLeaderboardWriteClient.UpsertDocumentAsync<UserLeaderboard>("/userId", userLeaderboard);
            }
        }
        public async void RedisColdStart()
        {
            List<string> names = new List<string>() { "Autos", "Local", "Travel" };
            foreach (var setName in names)
            {
                var sqlQueryText = String.Format("SELECT * FROM c where IS_DEFINED(c.reputation.{0})", setName);

                var queryDefinition = new QueryDefinitionWithParams(sqlQueryText);
                var responses = this.cosmosDbReadClient.QueryAsync<UserProfile>(query: queryDefinition, partitionKey: "/UserId");

                await foreach (var result in responses)
                {
                    string userId = result.UserId;
                    float points = result.ReputationBySegment[setName].PointValue;
                    redisWriteClient.AddtoSet(setName, userId, points);
                }
            }
        }

    }
}
