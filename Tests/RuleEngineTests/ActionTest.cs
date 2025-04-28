// <copyright file="RuleEngineTestUtils.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace RuleEngineTests
{
    using Common.Models.Enums;
    using Common.Utils;
    using CosmosDB.Models;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RuleEngine.Actions;
    using RuleEngineTest;
    using RulesEngine.Models;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class ActionTest
    {
        [TestMethod]
        public void DetermineActionTest()
        {
            var ruleResult = new RuleResultTree()
            {
                Inputs = new Dictionary<string, object>
                {
                    { "Action", "IncrementReputation" },
                }
            };

            var actionfactory = RuleEngineTestUtils.ActionFactory;
            var action = actionfactory.DetermineAction(ruleResult);
            Assert.IsInstanceOfType(action, typeof(IncrementReputationAction));
        }

        [TestMethod]
        public void ExecuteIncrementAdditionActionTest()
        {
            var userProfile = RuleEngineTestUtils.UserProfile;
            var segment = SegmentType.Autos;

            var activity = new ReputationActivity
            {
                Segment = segment.ToString(),
                ActivityType = ActivityType.Comment.ToString()
            };

            
            var ruleResult = new RuleResultTree()
            {
                Inputs = new Dictionary<string, object>
                {
                    { "Action", "IncrementReputation" },
                    { "Amount", 10 },
                }
            };

            var action = new IncrementReputationAction();
            action.Execute(
                userProfile: userProfile,
                activity: activity,
                segment: segment,
                ruleResult: ruleResult);

            Assert.AreEqual(userProfile.ReputationBySegment?.GetKeyValueOrNull(segment.ToString())?.PointValue, 10);
        }

        [TestMethod]
        public void ExecuteBadgeAdditionActionTest()
        {
            var userProfile = RuleEngineTestUtils.UserProfile;
            var segment = SegmentType.Autos;

            var activity = new ReputationActivity
            {
                Segment = segment.ToString(),
                ActivityType = ActivityType.Comment.ToString()
            };

            var ruleResult = new RuleResultTree()
            {
                Inputs = new Dictionary<string, object>
                {
                    { "Action", "AddBadge" },
                    { "BadgeType", "Silver" },
                }
            };

            var action = new BadgeAdditionAction();
            action.Execute(
                userProfile: userProfile,
                activity: activity,
                segment: segment,
                ruleResult: ruleResult);

            var addedBadge = userProfile.ReputationBySegment?.GetKeyValueOrNull(segment.ToString())?.Badges?.FirstOrDefault();
            Assert.IsTrue(addedBadge?.Name == "Silver");
        }

        [TestMethod]
        public void ExecuteUpdateLevelActionTest()
        {
            var userProfile = RuleEngineTestUtils.UserProfile;
            var segment = SegmentType.Autos;

            var activity = new ReputationActivity
            {
                Segment = segment.ToString(),
                ActivityType = ActivityType.Comment.ToString()
            };

            var ruleResult = new RuleResultTree()
            {
                Inputs = new Dictionary<string, object>
                {
                    { "Action", "UpdateLevel" },
                    { "NewLevel", "2" },
                    { "PointsForNextLevel", "10" },
                }
            };

            var action = new UpdateLevelAction();
            action.Execute(
                userProfile: userProfile,
                activity: activity,
                segment: segment,
                ruleResult: ruleResult);

            var userLevel = userProfile.ReputationBySegment?.GetKeyValueOrNull(segment.ToString())?.Level;
            Assert.AreEqual(userLevel?.CurrentLevel, (uint)2);
        }
    }
}