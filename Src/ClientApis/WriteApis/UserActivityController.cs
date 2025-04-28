// <copyright file="UserActivityController.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace WriteApis
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Common.Constants;
    using Common.Models.Responses;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.EventHubs;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;
    using UserActivityProcessor;

    public class UserActivityController
    {
        private readonly IUserActivityService userActivityService;

        public UserActivityController(IUserActivityService userActivityService)
        {
            this.userActivityService = userActivityService;
        }

        /// <summary>
        /// log newly activity data into blob storage and push as new event in event hub.
        /// </summary>
        /// <param name="req">http request.</param>
        /// <param name="outputEvents">event hub.</param>
        /// <param name="logger">logger.</param>
        /// <returns>A <see cref="Task"/> IBaseResponse.</returns>
        [FunctionName(FunctionNames.UserActivity)]
        public async Task<IBaseResponse> Activity(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "delete", Route = null)] HttpRequest req,
            [EventHub(ApiConstants.EventHubName, Connection = ApiConstants.EventHubConnStrName)] IAsyncCollector<string> outputEvents,
            ILogger logger)
        {
            var startTime = Stopwatch.StartNew();

            logger.LogInformation($"Process User activity {req.Method} event");

            var response = await this.userActivityService.ProcessAddActivityRequest(req, outputEvents, ApiConstants.ActivityStorageContainerName, logger);

            logger.LogInformation($"Processing time {startTime.ElapsedMilliseconds} msecs");

            return response;
        }

        /// <summary>
        /// Process event hub activity events.
        /// Use exponential retry logic in case of function failure with max retry count = 5.
        /// First time retry after 30 secs and keep on increasing interval time for each call (max time gap is 10 mins).
        /// </summary>
        /// <param name="events">event hub events.</param>
        /// <param name="log">logger.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [FunctionName(FunctionNames.ProcessUgcEvents)]
        [ExponentialBackoffRetry(maxRetryCount: ApiConstants.MaxRetryCount, minimumInterval: "00:00:30", maximumInterval: "00:10:00")]
        public async Task ProcessActivities(
            [EventHubTrigger(ApiConstants.EventHubName, Connection = ApiConstants.EventHubConnStrName)] EventData[] events,
            ExecutionContext executionContext,
            ILogger log)
        {
            var startTime = Stopwatch.StartNew();

            log.LogInformation("Process Event hub events");

            await this.userActivityService.ProcessEvents(events, executionContext, log);

            log.LogInformation($"Processing time {startTime.ElapsedMilliseconds} msecs");
        }
    }
}
