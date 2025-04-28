// <copyright file="IncrementReputationAction.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace RuleEngine.Actions
{
    using Common.Models.Enums;
    using Common.Utils;
    using CosmosDB.Models;
    using RulesEngine.Models;
    using System;
    using System.Collections.Generic;

    public class IncrementReputationAction : IAction
    {
        public void Execute(
            UserProfile userProfile,
            ReputationActivity activity,
            SegmentType segment,
            RuleResultTree ruleResult)
        {
            var rawIncrementAmount = ruleResult.Inputs["Amount"].ToString();
            var activityType = EnumParserUtil.GetEnum<ActivityType>(activity.ActivityType);

            if (!int.TryParse(rawIncrementAmount, out var incrementAmount)
                || activityType == ActivityType.Unknown)
            {
                throw new Exception("Insufficient info available for action");
            }

            var reputationDict = userProfile.ReputationBySegment
                ?? new Dictionary<string, Reputation>();
            var segmentReputation = reputationDict.GetKeyValueOrNull(segment.ToString())
                ?? new Reputation();
            var activityStats = segmentReputation.ActivityStats
                ?? new Dictionary<string, ActivityStats>();

            var activityTypeString = activityType.ToString();
            var stats = activityStats.GetKeyValueOrNull(activityTypeString)
                ?? new ActivityStats();
            stats.PointValue += incrementAmount;
            activityStats[activityTypeString] = stats;

            segmentReputation.ActivityStats = activityStats;
            segmentReputation.PointValue += incrementAmount;
            reputationDict[segment.ToString()] = segmentReputation;
            userProfile.ReputationBySegment = reputationDict;
        }
    }
}
