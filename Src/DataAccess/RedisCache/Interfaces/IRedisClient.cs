using RedisCache.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisCache.Interfaces
{
    public interface IRedisClient
    {
        Task<RedisConnection> CreateRedisConnection();
    }
}
