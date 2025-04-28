using Common.Models.Enums;
using Common.Utils;
using Microsoft.AspNetCore.Mvc;
using WebAppLeaderboard.Models;
using LeaderboardProcessor;
using CosmosDB.Interfaces;
using RedisCache.Interfaces;
using Common.Models.Requests;
using GetReputationProcessor;
using Configs;
using CosmosDB.Services.Data;
using RedisCache.Services;

namespace WebAppLeaderboard.Controllers
{
    public class UserActivityController : Controller
    {
        private readonly ILogger<UserActivityController> _logger;
        private IMultiLeaderboardProcessor multiLeaderboardProcessor;
        //private readonly IRedisWriteClient redisWriteClient;
        //private readonly IRedisReadClient redisReadClient;
        private readonly IConfiguration _config;

        public UserActivityController(ILogger<UserActivityController> logger, IConfiguration configuration)
        {
            
            _config = configuration;
            _logger = logger;

        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Create(string userId)
        {
            if (userId == null || null == Request.Form["Segment"].ToString())
            {
                return View();
            }
            var request = new ActivityRequest()
            {
                UserId = userId,
                ActivityType = Request.Form["Leaderboard"].ToString(),
                SegmentId = Request.Form["Segment"].ToString(),
                TimeStamp = DateTime.Now,
            };

            // Logging via Sessions - Not recommended
            var rawSegment = Request.Form["Segment"].ToString();
            var points = Request.Form["Points"];
            var leaderboard = Request.Form["Leaderboard"];
            //var leaderboard=
            var segment = EnumParserUtil.GetEnum<Segment>(rawSegment);
            //var serialisedObj = HttpContext.Session.GetString("UserModel");
            UserProfileModel model = new UserProfileModel(userId, segment);
            /*if (serialisedObj == null)
            {
                model = new UserProfileModel()
                {
                    Activities = new List<ActivityRequest>(),
                };
            }
            else
            {
                model = JsonConvertUtil.DeserializeObject<UserProfileModel>(serialisedObj);
            }*/
            model.Activities = new List<ActivityRequest>();
            model.Activities.Insert(0, request);
            var cosmosConfig = new CosmosDbConfig();
            cosmosConfig.EndPointUrl = _config["Values:CosmosDbEndPointUrl"];
            cosmosConfig.PrimaryKey = _config["Values:CosmosDbPrimaryKey"];
            var cosmosDbUserLeaderboardReadClient = new CosmosDbUserLeaderboardReadClient(cosmosConfig);
            var cosmosDbUserLeaderboardWriteClient = new CosmosDbUserLeaderboardWriteClient(cosmosConfig);
            //var cosmosDbLeaderboardReadClient = new CosmosDbLeaderboardReadClient(cosmosConfig);

            var redisReadClient = new RedisReadClient();
            redisReadClient.connectionString = _config["Values:RedisConnectionString"];
            redisReadClient.redisConnection = redisReadClient.CreateRedisConnection();
            var redisWriteClient = new RedisWriteClient();
            redisWriteClient.connectionString = _config["Values:RedisConnectionString"];
            redisWriteClient.redisConnection=redisWriteClient.CreateRedisConnection();

            this.multiLeaderboardProcessor = new MultiLeaderboardProcessor(cosmosDbUserLeaderboardReadClient, cosmosDbUserLeaderboardWriteClient);
            await this.multiLeaderboardProcessor.ProcessMultiLeaderboard(userId, float.Parse(points),  rawSegment, rawSegment+"/"+leaderboard,redisWriteClient, redisReadClient );
            return View("Create", model);
        }

        public async Task<IActionResult> List(string userId)
        {
            if (userId == null || null == Request.Form["Segment"].ToString())
            {
                return View();
            }
            var model = new UserProfileModel(userId);
            var cosmosConfig = new CosmosDbConfig();
            cosmosConfig.EndPointUrl = _config["Values:CosmosDbEndPointUrl"];
            cosmosConfig.PrimaryKey = _config["Values:CosmosDbPrimaryKey"];
            var cosmosDbUserLeaderboardReadClient = new CosmosDbUserLeaderboardReadClient(cosmosConfig);
            var cosmosDbUserLeaderboardWriteClient = new CosmosDbUserLeaderboardWriteClient(cosmosConfig);
            //var cosmosDbLeaderboardReadClient = new CosmosDbLeaderboardReadClient(cosmosConfig);
            this.multiLeaderboardProcessor = new MultiLeaderboardProcessor(cosmosDbUserLeaderboardReadClient, cosmosDbUserLeaderboardWriteClient);
            var userLeaderboard = await this.multiLeaderboardProcessor.GetLeaderboardByUserProfile(userId, "", "");
            model.dict = new Dictionary<string, KeyValuePair<float, long>>();
            var redisReadClient = new RedisReadClient();
            redisReadClient.connectionString = _config["Values:RedisConnectionString"];
            redisReadClient.redisConnection = redisReadClient.CreateRedisConnection();
            foreach (var leaderboard in userLeaderboard.Leaderboards)
            {
                var rank = await redisReadClient.GetRank(userId, leaderboard.Key);
                var tup = new KeyValuePair<float, long>(leaderboard.Value, rank+1);
                model.dict.Add(leaderboard.Key, tup);
            }
            return View("List", model);
        }
        public async Task<IActionResult> Leaderboard(string Leaderboard)
        {
            if (Leaderboard == null || null == Request.Form["Segment"].ToString())
            {
                return View();
            }
            var rawSegment = Request.Form["Segment"].ToString();
            var segment = EnumParserUtil.GetEnum<Segment>(rawSegment);
            var rawActivity = Request.Form["Leaderboard"].ToString();
            //var activity = EnumParserUtil.GetEnum<Segment>(rawActivity);
            var model = new LeaderboardModel();
            model.Segment = segment;
            model.Leaderboard = rawActivity;
            var redisReadClient = new RedisReadClient();
            redisReadClient.connectionString = _config["Values:RedisConnectionString"];
            redisReadClient.redisConnection = redisReadClient.CreateRedisConnection();
            
            Dictionary<string, double> Segtopn = await redisReadClient.GetTopn(rawSegment, 10);
            
            model.Segmenttopn = Segtopn;

            Dictionary<string, double> Acttopn = await redisReadClient.GetTopn(rawSegment+"/"+rawActivity, 5);

            model.Activitytopn = Acttopn;

            return View("Leaderboard", model);
        }

        public async Task<IActionResult> Rank(string userId)
        {
            if (userId == null || null == Request.Form["Segment"].ToString())
            {
                return View();
            }
            var rawSegment = Request.Form["Segment"].ToString();
            var segment = EnumParserUtil.GetEnum<Segment>(rawSegment);
            var rawActivity = Request.Form["Leaderboard"].ToString();
            //var activity = EnumParserUtil.GetEnum<Segment>(rawActivity);
            var model = new LeaderboardModel();
            model.Leaderboard = rawActivity;
            var redisReadClient = new RedisReadClient();
            redisReadClient.connectionString = _config["Values:RedisConnectionString"];
            redisReadClient.redisConnection = redisReadClient.CreateRedisConnection();
            model.UserId = userId;
            long Segmentrank = await redisReadClient.GetRank(userId, rawSegment);
            model.Segmentrank = Segmentrank+1;


            long Activityrank = await redisReadClient.GetRank(userId, rawSegment+"/"+rawActivity);
            model.Activityrank = Activityrank + 1;


            return View("Rank", model);
        }

        public async Task<IActionResult> CreatePrivate(string userId)
        {
            if (userId == null || null == Request.Form["Segment"].ToString())
            {
                return View();
            }
            var rawSegment = Request.Form["Segment"].ToString();
            var segment = EnumParserUtil.GetEnum<Segment>(rawSegment);
            var rawActivity = Request.Form["Leaderboard"].ToString();
            var model = new PrivateLeaderboard(userId, segment);
            var u1 = Request.Form["UserId1"].ToString();
            var u2 = Request.Form["UserId2"].ToString();
            var u3 = Request.Form["UserId3"].ToString();
            var u4 = Request.Form["UserId4"].ToString();

            List<string> users = new List<string>() { userId, u1, u2, u3, u4 };

            var name= Request.Form["PrivateLeaderboardName"].ToString();

            var cosmosConfig = new CosmosDbConfig();
            cosmosConfig.EndPointUrl = _config["Values:CosmosDbEndPointUrl"];
            cosmosConfig.PrimaryKey = _config["Values:CosmosDbPrimaryKey"];
            var cosmosDbUserLeaderboardReadClient = new CosmosDbUserLeaderboardReadClient(cosmosConfig);
            var cosmosDbUserLeaderboardWriteClient = new CosmosDbUserLeaderboardWriteClient(cosmosConfig);
            //var cosmosDbLeaderboardReadClient = new CosmosDbLeaderboardReadClient(cosmosConfig);

            var redisReadClient = new RedisReadClient();
            redisReadClient.connectionString = _config["Values:RedisConnectionString"];
            redisReadClient.redisConnection = redisReadClient.CreateRedisConnection();
            var redisWriteClient = new RedisWriteClient();
            redisWriteClient.connectionString = _config["Values:RedisConnectionString"];
            redisWriteClient.redisConnection = redisWriteClient.CreateRedisConnection();

            var privateLaderboardProcessor = new PrivateLeaderboardProcessor(cosmosDbUserLeaderboardReadClient, cosmosDbUserLeaderboardWriteClient, redisReadClient, redisWriteClient);
            await privateLaderboardProcessor.NewPrivateLeaderboardOnboarding(users, rawSegment+"/"+rawActivity+"/"+ name,rawSegment,rawActivity);
            return View("CreatePrivate", model);
        }

        public async Task<IActionResult> ViewPrivate(string userId)
        {
            if (userId == null || null == Request.Form["Segment"].ToString())
            {
                return View();
            }
            var rawSegment = Request.Form["Segment"].ToString();
            var segment = EnumParserUtil.GetEnum<Segment>(rawSegment);
            var rawActivity = Request.Form["Leaderboard"].ToString();
            var model = new PrivateLeaderboard(userId, segment);
            model.Leaderboard = rawActivity;
            model.Segment = segment;
            var cosmosConfig = new CosmosDbConfig();
            cosmosConfig.EndPointUrl = _config["Values:CosmosDbEndPointUrl"];
            cosmosConfig.PrimaryKey = _config["Values:CosmosDbPrimaryKey"];
            var cosmosDbUserLeaderboardReadClient = new CosmosDbUserLeaderboardReadClient(cosmosConfig);
            var cosmosDbUserLeaderboardWriteClient = new CosmosDbUserLeaderboardWriteClient(cosmosConfig);
            //var cosmosDbLeaderboardReadClient = new CosmosDbLeaderboardReadClient(cosmosConfig);

            var redisReadClient = new RedisReadClient();
            redisReadClient.connectionString = _config["Values:RedisConnectionString"];
            redisReadClient.redisConnection = redisReadClient.CreateRedisConnection();
            var redisWriteClient = new RedisWriteClient();
            redisWriteClient.connectionString = _config["Values:RedisConnectionString"];
            redisWriteClient.redisConnection = redisWriteClient.CreateRedisConnection();

            var privateLaderboardProcessor = new PrivateLeaderboardProcessor(cosmosDbUserLeaderboardReadClient, cosmosDbUserLeaderboardWriteClient, redisReadClient, redisWriteClient);

            model.leaderboards = await privateLaderboardProcessor.ViewPrivateLeaderboard(userId, rawSegment, rawActivity);
            

            return View("ViewPrivate", model);
        }

    }
}
