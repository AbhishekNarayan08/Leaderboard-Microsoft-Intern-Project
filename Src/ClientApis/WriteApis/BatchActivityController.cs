// <copyright file="BatchActivityController.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace WriteApis
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using BulkActivityProcessor;
    using Common.Constants;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;

    public class BatchActivityController
    {
        // trigger bulk activity function app once in a day.
        private const string Occurrence = "0 0 0 * * *";
        private readonly IBulkActivityService bulkActivityService;

        public BatchActivityController(IBulkActivityService bulkActivityService)
        {
            this.bulkActivityService = bulkActivityService;
        }

        /// <summary>
        /// process segment bulk activities in batches.
        /// </summary>
        /// <param name="myTimer">TimerInfo</param>
        /// <param name="logger">logger</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [FunctionName(FunctionNames.ProcessBulkEvents)]
        public async Task ProcessBulkEvents(
            [TimerTrigger(Occurrence)] TimerInfo myTimer,
            ExecutionContext context,
            ILogger logger)
        {
            var startTime = Stopwatch.StartNew();

            var response = await this.bulkActivityService.ProcessActivitiesInBatches(
                context,
                ApiConstants.SegmentOnboardActivitiesContainerName,
                logger);

            logger.LogInformation($"Bulk API response {response}. Processing time {startTime.ElapsedMilliseconds} msecs");
        }
    }
}
