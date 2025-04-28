using CosmosDB.Interfaces;
using CosmosDB.Models;
using CosmosDB.Services.Data;
using RedisCache.Interfaces;

namespace LeaderboardProcessor
{
    public class MultiLeaderboardProcessor : IMultiLeaderboardProcessor
    {
        private readonly ICosmosDbUserLeaderboardReadClient cosmosDbUserLeaderboardReadClient;
        private readonly ICosmosDbUserLeaderboardWriteClient cosmosDbUserLeaderboardWriteClient;


        public MultiLeaderboardProcessor(ICosmosDbUserLeaderboardReadClient cosmosDbUserLeaderboardReadClient, 
            ICosmosDbUserLeaderboardWriteClient cosmosDbUserLeaderboardWriteClient)
        {
            this.cosmosDbUserLeaderboardReadClient = cosmosDbUserLeaderboardReadClient;
            this.cosmosDbUserLeaderboardWriteClient = cosmosDbUserLeaderboardWriteClient;
        }

        public async Task<UserLeaderboard> GetLeaderboardByUserProfile(string userId, string Segment, string leaderboard)
        {
            var parameterizedQuery = new QueryDefinitionWithParams($@"SELECT *
                    FROM UserLeaderboards user
                    WHERE user.userId = @userId").
                   WithParameter("@userId",userId );

           
            var response = await cosmosDbUserLeaderboardReadClient.QuerySinglePageAsync<UserLeaderboard>(
                query: parameterizedQuery, partitionKey: "/userId");

            if(response == null || response.Count == 0)
            {
                var userLeaderboard = new UserLeaderboard();
                userLeaderboard.UserId = userId;
                userLeaderboard.Leaderboards = new Dictionary<string, float>();
                if (Segment != "")
                {
                    userLeaderboard.Leaderboards.Add(Segment, 0);
                }
                    
                if (leaderboard != "")
                {
                    userLeaderboard.Leaderboards.Add(leaderboard, 0);
                }
                    
                return userLeaderboard;
            }

            return response.Resource.FirstOrDefault();
        }

        public async Task ProcessMultiLeaderboard(string userId,  float points, string segment, string leaderboard,
            IRedisWriteClient redisWriteClient, IRedisReadClient redisReadClient)
        {
            var userLeaderboard = await GetLeaderboardByUserProfile(userId, segment, leaderboard);
            var multiLeaderboards = userLeaderboard.Leaderboards;
            if (multiLeaderboards.ContainsKey(leaderboard))
            {
                multiLeaderboards[leaderboard] += points;
            }
            else
            {
                multiLeaderboards.Add(leaderboard, points);
            }
            if (multiLeaderboards.ContainsKey(segment))
            {
                multiLeaderboards[segment] += points;
            }
            else
            {
                multiLeaderboards.Add(segment, points);
            }

            RankProcessor rankProcessor = new(redisWriteClient, redisReadClient);
            rankProcessor.processLeaderboard(userId, multiLeaderboards[segment], segment);

            rankProcessor.processLeaderboard(userId, multiLeaderboards[leaderboard], leaderboard);

            var privateLeaderboardProcesssor = new PrivateLeaderboardProcessor(cosmosDbUserLeaderboardReadClient, cosmosDbUserLeaderboardWriteClient, redisReadClient, redisWriteClient);
            privateLeaderboardProcesssor.ProcessPrivateLeaderboards(userId, ref multiLeaderboards, segment, leaderboard, points, rankProcessor);

            userLeaderboard.Id = userId;
            BackupProcessor backupProcessor = new(this.cosmosDbUserLeaderboardWriteClient);
            userLeaderboard.Leaderboards = multiLeaderboards;
            await backupProcessor.UpdateCosmos(userLeaderboard);
        }
    }
}
