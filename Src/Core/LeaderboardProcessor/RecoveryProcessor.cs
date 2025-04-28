using CosmosDB.Interfaces;
using CosmosDB.Models;
using CosmosDB.Services.Data;
using RedisCache.Interfaces;

namespace LeaderboardProcessor
{
    public class RecoveryProcessor
    {
        public IRedisWriteClient redisWriteClient;
        public ICosmosDbUserLeaderboardReadClient cosmosDbUserLeaderboardReadClient;
        public RecoveryProcessor(IRedisWriteClient redisWriteClient, ICosmosDbUserLeaderboardReadClient cosmosDbUserLeaderboardReadClient)
        {
            this.redisWriteClient = redisWriteClient;
            this.cosmosDbUserLeaderboardReadClient = cosmosDbUserLeaderboardReadClient;
        }
        //Query all the users and all of their leaderboards and add them to redis
        public async void recover()
        {

            var parameterizedQuery = new QueryDefinitionWithParams($@"SELECT *
                    FROM UserLeaderboards user");


            var responses = cosmosDbUserLeaderboardReadClient.QueryAsync<UserLeaderboard>(
                query: parameterizedQuery, partitionKey: "/userId");

            await foreach (var response in responses)
            {
                string userId = response.UserId;
                Dictionary<string, float> leaderboards = response.Leaderboards;
                foreach (var item in leaderboards)
                {
                    string setName = item.Key;
                    float points = item.Value;
                    this.redisWriteClient.AddtoSet(setName, userId, points);
                }
            }
        }
        //to recover a single leaderboard, we query all the users if the leaderboard is present in their list of concerned leaderboards and then add the user entry to redis

        public async void recoverLeaderboard(string leaderboard)
        {
            var parameterizedQuery = new QueryDefinitionWithParams($@"SELECT *
                    FROM UserLeaderboards user");


            var responses = cosmosDbUserLeaderboardReadClient.QueryAsync<UserLeaderboard>(
                query: parameterizedQuery, partitionKey: "/userId");

            await foreach (var response in responses)
            {
                string userId = response.UserId;
                Dictionary<string, float> leaderboards = response.Leaderboards;
                if (leaderboards.ContainsKey(leaderboard))
                {
                    float points = leaderboards[leaderboard];
                    this.redisWriteClient.AddtoSet(leaderboard, userId, points);
                }
            }
        }
    }
}
