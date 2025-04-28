// <copyright file="UpdateLevelAction.cs" company="Microsoft">
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

    public class UpdateLevelAction : IAction
    {
        public void Execute(
            UserProfile userProfile,
            ReputationActivity activity,
            SegmentType segment,
            RuleResultTree ruleResult)
        {
            var rawLevel = ruleResult.Inputs["NewLevel"]?.ToString();

            if (!uint.TryParse(rawLevel, out var newLevel))
            {
                throw new Exception("Insufficient info available for action");
            }

            var reputationDict = userProfile.ReputationBySegment
                ?? new Dictionary<string, Reputation>();
            var segmentReputation = reputationDict.GetKeyValueOrNull(segment.ToString())
                ?? new Reputation();

            var level = new Level
            {
                CurrentLevel = newLevel,
            };

            // Optional param
            var rawTarget = ruleResult.Inputs["PointsForNextLevel"]?.ToString();
            if (uint.TryParse(rawTarget, out var newTarget))
            {
                level.PointsAtNextMilestone = newTarget;
            }

            segmentReputation.Level = level;
            reputationDict[segment.ToString()] = segmentReputation;
            userProfile.ReputationBySegment = reputationDict;
        }
    }
}
