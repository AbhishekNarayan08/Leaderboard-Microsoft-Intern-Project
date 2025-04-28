using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RedisCache.Services;
using RedisCache.Interfaces;

namespace LeaderboardAPIs
{
    public class DisplayController
    {
        private readonly IRedisReadClient redisReadClient;
        public DisplayController(IRedisReadClient redisReadClient)
        {
            this.redisReadClient = redisReadClient;
        }
        [FunctionName("DisplayLeaderboards")]
        public async Task<IActionResult> DisplayLeaderboards(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string leaderboard = req.Query["leaderboard"];
            string n = req.Query["n"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            leaderboard = leaderboard ?? data?.leaderboard;
            n = n ?? data?.n;
            string responseMessage = string.IsNullOrEmpty(leaderboard)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {leaderboard}. This HTTP triggered function executed successfully.";
            int nval = int.Parse(n);
            var list = await this.redisReadClient.GetTopn(leaderboard, nval);
            Console.Write(list);
            return new OkObjectResult(responseMessage);
        }
    }
}
