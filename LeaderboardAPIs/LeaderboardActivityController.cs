
namespace LeaderboardAPIs
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using CosmosDB.Interfaces;
    using RedisCache.Interfaces;
    using LeaderboardProcessor;

    public class LeaderboardActivityController
    {
        private readonly ICosmosDbUserLeaderboardReadClient cosmosDbUserLeaderboardReadClient;
        private readonly IMultiLeaderboardProcessor multiLeaderboardProcessor;
        private readonly IRedisWriteClient redisWriteClient;
        private readonly IRedisReadClient redisReadClient;
        private readonly ICosmosDbUserLeaderboardWriteClient cosmosDbUserLeaderboardWriteClient;

        public LeaderboardActivityController(ICosmosDbUserLeaderboardReadClient cosmosDbUserLeaderboardReadClient, 
            IMultiLeaderboardProcessor multiLeaderboardProcessor,
            IRedisReadClient redisReadClient, IRedisWriteClient redisWriteClient,
            ICosmosDbUserLeaderboardWriteClient cosmosDbUserLeaderboardWriteClient)
        {
            this.cosmosDbUserLeaderboardReadClient = cosmosDbUserLeaderboardReadClient;
            this.multiLeaderboardProcessor = multiLeaderboardProcessor;
            this.redisWriteClient = redisWriteClient;
            this.redisReadClient = redisReadClient;
            this.cosmosDbUserLeaderboardWriteClient = cosmosDbUserLeaderboardWriteClient;
        }

        [FunctionName("LeaderboardActivity")]
        public async Task<IActionResult> LeaderboardActivity(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            
            string userId = req.Query["userId"];
            string points = req.Query["points"];
            string segment = req.Query["segment"];
            string leaderboard = req.Query["leaderboard"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            userId = userId ?? data?.userId;
            points = points ?? data?.points;
            segment = segment ?? data?.segment;
            leaderboard = leaderboard ?? data?.leaderboard; 


            string responseMessage = string.IsNullOrEmpty(userId)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {userId}. This HTTP triggered function executed successfully.";
            await this.multiLeaderboardProcessor.ProcessMultiLeaderboard(userId, float.Parse(points), segment, leaderboard,
                this.redisWriteClient,this.redisReadClient);
            return new OkObjectResult(responseMessage);
        }
    }
}
