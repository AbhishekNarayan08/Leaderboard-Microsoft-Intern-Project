// <copyright file="BadgeAdditionAction.cs" company="Microsoft">
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

    public class BadgeAdditionAction : IAction
    {
        public void Execute(
            UserProfile userProfile,
            ReputationActivity activity,
            SegmentType segment,
            RuleResultTree ruleResult)
        {
            var badge = ruleResult.Inputs["BadgeType"]?.ToString();
            if (string.IsNullOrEmpty(badge))
            {
                throw new Exception("Insufficient info available for action");
            }

            var reputationDict = userProfile.ReputationBySegment
                ?? new Dictionary<string, Reputation>();
            var segmentReputation = reputationDict.GetKeyValueOrNull(segment.ToString())
                ?? new Reputation();
            var badges = segmentReputation.Badges
                ?? new List<Badge>();

            badges.Add(new Badge 
            {
                Name = badge,
                CreationDate = DateTime.Now,
            });
            segmentReputation.Badges = badges;
            reputationDict[segment.ToString()] = segmentReputation;
            userProfile.ReputationBySegment = reputationDict;
        }
    }
}
