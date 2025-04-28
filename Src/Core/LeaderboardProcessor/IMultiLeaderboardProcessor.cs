using CosmosDB.Interfaces;
using CosmosDB.Models;
using RedisCache.Interfaces;

namespace LeaderboardProcessor
{
    public interface IMultiLeaderboardProcessor
    {
        public Task<UserLeaderboard> GetLeaderboardByUserProfile(string userId, string segment, string leaderboard);

        public Task ProcessMultiLeaderboard(string userId, float points, string segment, string leaderboard,
            IRedisWriteClient redisWriteClient, IRedisReadClient redisReadClient);
    }
}
