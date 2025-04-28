namespace LeaderboardProcessor
{
    using CosmosDB.Models;
    using RedisCache.Interfaces;
    using RedisCache.Services;
    using System;
    using System.Collections.Generic;

    public class RankProcessor
    {
        private readonly IRedisReadClient redisReadClient;
        private readonly IRedisWriteClient redisWriteClient;

        public RankProcessor(IRedisWriteClient redisWriteClient, IRedisReadClient redisReadClient)
        {
            this.redisWriteClient = redisWriteClient;
            this.redisReadClient = redisReadClient;
        }
        public void processLeaderboard(string userId, float points, string LName)
        {
            //var Lname = leaderboard.LeaderboardName;
            //var Lname = segment;
            //string userId = user.UserId;
            //var longPoint = float.Parse(points);
            this.redisWriteClient.AddtoSet(LName, userId, points);
        }
        public void GetRank(string userId, string Lname)
        {
            //var Lname = leaderboard.LeaderboardName;
            //string userId = user.UserId;
            Task<long> rank = this.redisReadClient.GetRank(userId, Lname);
            //Console.WriteLine("Rank of {0} in {1} is {2}\n", userId , Lname, rank);
        }
        public void GetTopn(string Lname, int n)
        {
            var list = this.redisReadClient.GetTopn(Lname, n);
            Console.Write(list);
            /*
            foreach(KeyValuePair<string, double> item in list)
            {
                Console.WriteLine("{0} : {1}\n", item.Key, item.Value);
            }
            */
        }
        public void getUserInfo(UserProfile user)
        {
            string userId = user.UserId;
            IDictionary<string, Reputation> ReputationBySegment = user.ReputationBySegment;
            UserProfileAttributes Attributes = user.Attributes;
            string AboutMe = Attributes.AboutMe;
            string FollowerCount = Attributes.FollowerCount;
            //display the details
            Console.WriteLine("About Me: {0}\n", AboutMe);
            Console.WriteLine("FollowerCount: {0}\n", FollowerCount);
        }
    }
}
