// <copyright file="RuleRepository.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace RuleEngine.Rules
{
    using AzureStorage;
    using Common.Constants;
    using Common.Models.Enums;
    using Common.Utils;
    using CosmosDB.Models;
    using RuleEngine.Rules.Snapshots;
    using RulesEngine.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class RuleRepository : IRuleRepository
    {
        private readonly IBlobStorageContext blobStorageContext;

        public RuleRepository(IBlobStorageContext blobStorageContext)
        {
            this.blobStorageContext = blobStorageContext;
        }

        public async Task<List<Workflow>?> GetLatestRulesForSegment(
            SegmentType segment,
            bool usecache = false)
        {
            if (segment == SegmentType.Unknown)
            {
                return null;
            }

            var rulesetSnapshots = await this.FetchSegmentRulesetSnapshots(segment);

            if (rulesetSnapshots == null
                || rulesetSnapshots.Count == 0)
            {
                return null;
            }

            rulesetSnapshots = rulesetSnapshots.OrderBy(snapshot => snapshot.Metadata.CreationDateTime).ToList();
            return rulesetSnapshots.Last().Workflows;
        }

        public async Task<List<Workflow>?> GetRulesForActivity(
            ReputationActivity activity,
            bool usecache = false)
        {
            var segmentType = EnumParserUtil.GetEnum<SegmentType>(activity.Segment);

            if (segmentType == SegmentType.Unknown
                || activity.TimeStamp == null)
            {
                return null;
            }

            var rulesetSnapshots = await this.FetchSegmentRulesetSnapshots(segmentType);

            if (rulesetSnapshots == null
                || rulesetSnapshots.Count == 0)
            {
                return null;
            }

            rulesetSnapshots = rulesetSnapshots.OrderBy(snapshot => snapshot.Metadata.CreationDateTime).ToList();
            var selectedRulesetSnapshot = this.PickRuleSnapshot(rulesetSnapshots, activity.TimeStamp.DateTime);

            return selectedRulesetSnapshot.Workflows;
        }

        protected virtual async Task<List<RulesetSnapshot>> FetchSegmentRulesetSnapshots(SegmentType segmentType)
        {
            var rulesetSnasphsots = new List<RulesetSnapshot>();

            var containerName = ContainerNames.RuleSets;
            var blobs = this.blobStorageContext.ListBlobs(
                containerName: containerName,
                prefix: segmentType.ToString());
            
            await foreach (var blob in blobs)
            {
                if (string.IsNullOrEmpty(blob.Name))
                {
                    continue;
                }

                var ruleSetSnapshot = await this.blobStorageContext.DownloadJSONObjectAsync<RulesetSnapshot>(
                    containerName: containerName,
                    blobPath: blob.Name);

                if (ruleSetSnapshot != null)
                {
                    rulesetSnasphsots.Add(ruleSetSnapshot);
                }
            }

            return rulesetSnasphsots;
        }

        /// <summary>
        /// Select latest available ruleset version that was created before the activity
        /// </summary>
        /// <param name="rulesetSnapshots">Non-empty list of available ruleset snapshots.</param>
        /// <param name="activityDateTime">creation time of activity</param>
        /// <returns>A <see cref="RulesetSnapshot"/>selected suleset snapshot</returns>
        private RulesetSnapshot PickRuleSnapshot(
            List<RulesetSnapshot> rulesetSnapshots,
            DateTime activityDateTime)
        {
            var snapshotTimestamps = rulesetSnapshots.Select(snapshot => snapshot.Metadata.CreationDateTime).ToList();
            var index = -1;

            // If the activity was created before any ruleses were available, pick the oldest one.
            if (activityDateTime < snapshotTimestamps.First())
            {
                index = 0;
            }
            // If the activity was created after the latest ruleset snapshot, pick it straight away.
            else if (activityDateTime > snapshotTimestamps.Last())
            {
                index = snapshotTimestamps.Count - 1;
            }
            else
            { 
                index = snapshotTimestamps.BinarySearch(activityDateTime);
                if (index < 0)
                {
                    // If item is not found in the list; binary search returns a negative number that is the bitwise complement
                    // of the index of the next element that is larger than item
                    // see ref: https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1.binarysearch?view=net-6.0
                    index = (~index) - 1;
                }
            }

            return rulesetSnapshots[index];
        }
    }
}
