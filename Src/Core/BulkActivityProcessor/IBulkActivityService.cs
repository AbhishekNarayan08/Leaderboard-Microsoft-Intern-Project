// <copyright file="IBulkActivityService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace BulkActivityProcessor
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Models.Requests;
    using CosmosDB.Models;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;

    public interface IBulkActivityService
    {
        public List<ActivityRequest> GetValidateActivities(
            List<string> events,
            ILogger log);

        public Task<IDictionary<UserProfile, List<ReputationActivity>>> ApplyRulesOnActivities(
            List<ActivityRequest> actityRequests,
            ILogger log);

        public Task<List<ActivityRequest>> MoveActivityBlobs(
            List<ActivityRequest> actityRequests,
            string sourceContainer,
            string destinationContainer,
            ILogger log);

        public Task<int> UpdateBatchActivitiesInDb(
            KeyValuePair<UserProfile, List<ReputationActivity>> userProfileActivities,
            ExecutionContext context,
            ILogger log);

        public Task<bool> ProcessActivitiesInBatches(
            ExecutionContext context,
            string containerName,
            ILogger log);
    }
}
