using RedisCache.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisCache.Interfaces
{
    public interface IRedisWriteClient
    {
        Task<RedisConnection> CreateRedisConnection();
        Task AddtoSet(string setName, string userId, float points);
        void CleanCache();
        string connectionString { get; set; }
        Task<RedisConnection> redisConnection { get; set; }
    }
}
