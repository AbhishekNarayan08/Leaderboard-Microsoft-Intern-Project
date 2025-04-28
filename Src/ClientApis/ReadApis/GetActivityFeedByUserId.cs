// <copyright file="GetActivityFeedByUserId.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ReadApis
{
    using System.Threading.Tasks;
    using Common.Constants;
    using GetActivityFeedProcessor;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;
    using ReadApis.Factories;

    public class GetActivityFeedByUserId
    {
        private readonly IGetActivityFeedService getActivityFeedService;
        private readonly IActivityFeedServiceFactory activityFeedServiceFactory;

        public GetActivityFeedByUserId(IActivityFeedServiceFactory activityFeedServiceFactory)
        {
            this.activityFeedServiceFactory = activityFeedServiceFactory;
            this.getActivityFeedService = this.activityFeedServiceFactory.GetActivityFeedService<GetActivityFeedByUserIdService>();
        }

        [FunctionName(FunctionNames.GetActivityFeedByUserId)]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger logger)
        {
            logger.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];
            var output = await this.getActivityFeedService.GetActivityFeed(req, logger);

            return new OkObjectResult(output);
        }
    }
}
