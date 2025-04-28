// <copyright file="RedisReadClient.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace RedisCache.Services
{

    using RedisCache.Interfaces;
    using StackExchange.Redis;
    using System.Collections.Generic;
    using System.Text.Json;

    public class RedisReadClient : RedisClient, IRedisReadClient
    {
        public string connectionString { get; set; }
        public Task<RedisConnection> redisConnection { get; set; }
        public async Task<RedisConnection> CreateRedisConnection()
        {
            return await RedisConnection.InitializeAsync(this.connectionString);
        }
        public async Task<long> GetRank(string userId, string setName)
        {
            //IDatabase database = Connection.GetDatabase();
            //long rank = (long)database.SortedSetRank(setName, userId);
            RedisValue getMessageResult = await redisConnection.Result.BasicRetryAsync(async (db) => await db.SortedSetRankAsync(setName, userId,Order.Descending));
            long rank = JsonSerializer.Deserialize<long>(getMessageResult);
            //Console.WriteLine("Rank of {0} in {1} is {2}\n", userId, setName, rank);
            Console.WriteLine($" Rank of {userId} in {setName}: {getMessageResult}");
            return rank;
        }
        public async Task<Dictionary<string, double>> GetTopn(string setName, int n)
        {
            //IDatabase database = Connection.GetDatabase();
            //var list = database.SortedSetRangeByRankWithScores(setName, stop: n, order: Order.Descending);
            var getMessageResult = await redisConnection.Result.BasicRetryAsync(async (db) => await db.SortedSetRangeByRankWithScoresAsync(setName, stop: n, order: Order.Descending));

            Dictionary<string, double> topn = new Dictionary<string, double>();
            foreach (var item in getMessageResult)
            {
                topn.Add(item.Element, item.Score);
            }
            return topn;
        }
    }
}
