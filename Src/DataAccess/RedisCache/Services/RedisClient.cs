using Configs;
using RedisCache.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace RedisCache.Services
{
    public class RedisClient : IRedisClient
    {
        /*
        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            string connectionString = ConfigUtils.RetrieveVerifyConfigValue("RedisConnectionString");
            return ConnectionMultiplexer.Connect(connectionString);
        });
        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }
        */
        public string connectionString { get; set; }// = ConfigUtils.RetrieveVerifyConfigValue("RedisConnectionString");
        //private ConnectionMultiplexer Connection;// = ConnectionMultiplexer.Connect(connectionString);
        //public IDatabase database;
        public Task<RedisConnection> redisConnection { get; set; }
        public RedisClient()
        {
            //this.connectionString = ConfigUtils.RetrieveVerifyConfigValue("RedisConnectionString");
            //this.Connection = ConnectionMultiplexer.Connect(connectionString);
            //this.redisConnection = CreateRedisConnection();
            //this.database = Connection.GetDatabase();
        }



        public async Task<RedisConnection> CreateRedisConnection()
        {
            return await RedisConnection.InitializeAsync(this.connectionString);
        }
    }
}