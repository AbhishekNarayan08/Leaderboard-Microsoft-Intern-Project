using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Configs;
using RedisCache.Interfaces;
using StackExchange.Redis;
using RedisCache.Services;

namespace RedisCache.Services
{
    public class RedisWriteClient : RedisClient, IRedisWriteClient
    {
        public string connectionString { get; set; }
        public Task<RedisConnection> redisConnection { get; set; }
        public async Task<RedisConnection> CreateRedisConnection()
        {
            return await RedisConnection.InitializeAsync(this.connectionString);
        }
        public async Task AddtoSet(string setName, string userId, float points)
        {
            //IDatabase database = Connection.GetDatabase();
            //return  database.SortedSetAdd(setName, userId, points);
            RedisValue getMessageResult = await redisConnection.Result.BasicRetryAsync(async (db) => await db.SortedSetAddAsync(setName, userId, points));

        }
        //clean redis
        public async void CleanCache()
        {
            this.redisConnection.Dispose();
        }
    }
}
