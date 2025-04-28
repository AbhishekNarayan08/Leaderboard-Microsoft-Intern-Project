using CosmosDB.Interfaces;
using CosmosDB.Models;
using Microsoft.Azure.Documents;
using RedisCache.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaderboardProcessor
{
    public class PrivateLeaderboardProcessor
    {
        private readonly ICosmosDbUserLeaderboardReadClient cosmosDbUserLeaderboardReadClient;
        private readonly ICosmosDbUserLeaderboardWriteClient cosmosDbUserLeaderboardWriteClient;
        private readonly IRedisReadClient redisReadClient;
        private readonly IRedisWriteClient redisWriteClient;

        public PrivateLeaderboardProcessor(ICosmosDbUserLeaderboardReadClient cosmosDbUserLeaderboardReadClient, ICosmosDbUserLeaderboardWriteClient cosmosDbUserLeaderboardWriteClient,
            IRedisReadClient redisReadClient, IRedisWriteClient redisWriteClient)
        {
            this.cosmosDbUserLeaderboardReadClient = cosmosDbUserLeaderboardReadClient;
            this.cosmosDbUserLeaderboardWriteClient = cosmosDbUserLeaderboardWriteClient;
            this.redisReadClient = redisReadClient;
            this.redisWriteClient = redisWriteClient;
        }

        public async Task NewPrivateLeaderboardOnboarding(List<string> Users, string leaderboard, string segment, string activity)
        {
            MultiLeaderboardProcessor multiLeaderboardProcessor = new MultiLeaderboardProcessor(cosmosDbUserLeaderboardReadClient, cosmosDbUserLeaderboardWriteClient);
            string s = segment + "/" + activity;

            foreach (var user in Users)
            {
                float points = 0;
                UserLeaderboard userLeaderboard = await multiLeaderboardProcessor.GetLeaderboardByUserProfile(user, "", "");
                if(userLeaderboard.Leaderboards.ContainsKey(s))
                {
                    points = userLeaderboard.Leaderboards[s];
                }
                else
                {
                    userLeaderboard.Leaderboards.Add(s, 0);
                }
                userLeaderboard.Leaderboards.Add(leaderboard, points);
                userLeaderboard.Id = userLeaderboard.UserId;
                await cosmosDbUserLeaderboardWriteClient.UpsertDocumentAsync<UserLeaderboard>("/userId", userLeaderboard);
                await redisWriteClient.AddtoSet(leaderboard, userLeaderboard.UserId, points);

            }
        }

        public void ProcessPrivateLeaderboards(string userId, ref Dictionary<string, float> multileaderboards, string segment,
            string activity, float points, RankProcessor rankProcessor)
        {
            foreach (var leaderboard in multileaderboards)
            {
                var Lname = leaderboard.Key;
                string s = activity;
                if (Lname.Length > activity.Length + 1 && Lname.Substring(0, activity.Length) == s)
                {
                    multileaderboards[leaderboard.Key] = multileaderboards[s];
                    rankProcessor.processLeaderboard(userId, multileaderboards[leaderboard.Key], leaderboard.Key);
                }
            }
        }

        public async Task<List<KeyValuePair<string,Dictionary<string, double>>>> ViewPrivateLeaderboard(string userId, string segment, string activity)
        {
            MultiLeaderboardProcessor multiLeaderboardProcessor = new MultiLeaderboardProcessor(cosmosDbUserLeaderboardReadClient, cosmosDbUserLeaderboardWriteClient);
            UserLeaderboard userLeaderboard = await multiLeaderboardProcessor.GetLeaderboardByUserProfile(userId, "", "");
            var multileaderboards = userLeaderboard.Leaderboards;
            List<KeyValuePair<string, Dictionary<string, double>>> dicts = new List<KeyValuePair<string, Dictionary<string, double>>>();
            foreach (var leaderboard in multileaderboards)
            {
                Dictionary<string, double> dict = new Dictionary<string, double>();
                var Lname = leaderboard.Key;
                string s = segment + "/" + activity;
     
                if (Lname.Length>segment.Length+activity.Length+2 && Lname.Substring(0, segment.Length + activity.Length + 1) == s)
                {
                    dict = await redisReadClient.GetTopn(Lname, 10);
                    KeyValuePair<string,Dictionary<string,double>> keyValuePair = new KeyValuePair<string, Dictionary<string, double>>(Lname,dict);
                   
                    dicts.Add(keyValuePair);
                }
            }
            return dicts;
        }
    }
}
