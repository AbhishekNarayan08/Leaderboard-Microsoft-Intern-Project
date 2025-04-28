// <copyright file="IRedisReadClient.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace RedisCache.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using RedisCache.Services;
    public interface IRedisReadClient
    {
        Task<RedisConnection> CreateRedisConnection();
        Task<long> GetRank(string userId, string setName);
        Task<Dictionary<string, double>> GetTopn(string setName, int n);

        string connectionString { get; set; }
        Task<RedisConnection> redisConnection { get; set; }
    }
}
