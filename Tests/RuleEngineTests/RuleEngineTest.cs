// <copyright file="RuleEngineTest.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>


namespace RuleEngineTest
{
    using Common.Models.Enums;
    using Common.Utils;
    using CosmosDB.Models;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Linq;

    [TestClass]
    public class RuleEngineTest
    {
        [TestMethod]
        public void RuleEngineProcessPickLatestRulesTest()
        {
            var segment = SegmentType.Autos.ToString();
            var userProfile = RuleEngineTestUtils.UserProfile;
            var activity = new ReputationActivity
            {
                ActivityType = ActivityType.Like.ToString(),
                Segment = segment,
                TimeStamp = new DateTimeOffset(new DateTime(2022, 2, 2))
            };

            var ruleEngine = RuleEngineTestUtils.RuleEngine;
            ruleEngine.Process(
                userProfile: userProfile,
                activity: activity,
                logger: RuleEngineTestUtils.Logger);

            var addedBadge = userProfile.ReputationBySegment?.GetKeyValueOrNull(segment)?.Badges?.FirstOrDefault();
            Assert.IsTrue(addedBadge?.Name == "Silver");
        }

        [TestMethod]
        public void RuleEngineProcessPickOldestRulesTest()
        {
            var segment = SegmentType.Autos.ToString();
            var userProfile = RuleEngineTestUtils.UserProfile;
            var activity = new ReputationActivity
            {
                ActivityType = ActivityType.Comment.ToString(),
                Segment = segment,
                TimeStamp = new DateTimeOffset(new DateTime(2021, 2, 2))
            };

            var ruleEngine = RuleEngineTestUtils.RuleEngine;
            ruleEngine.Process(
                userProfile: userProfile,
                activity: activity,
                logger: RuleEngineTestUtils.Logger);

            var addedBadge = userProfile.ReputationBySegment?.GetKeyValueOrNull(segment)?.Badges?.FirstOrDefault();
            Assert.IsTrue(addedBadge?.Name == "Bronze");

            var userLevel = userProfile.ReputationBySegment?.GetKeyValueOrNull(segment.ToString())?.Level;
            Assert.AreEqual(userLevel?.CurrentLevel, (uint)2);
        }

        [TestMethod]
        public void RuleEngineProcessPickViaBinarySearchTest()
        {
            var segment = SegmentType.Autos.ToString();
            var userProfile = RuleEngineTestUtils.UserProfile;
            var activity = new ReputationActivity
            {
                ActivityType = ActivityType.Comment.ToString(),
                Segment = segment,
                TimeStamp = new DateTimeOffset(new DateTime(2021, 5, 25))
            };

            var ruleEngine = RuleEngineTestUtils.RuleEngine;
            ruleEngine.Process(
                userProfile: userProfile,
                activity: activity,
                logger: RuleEngineTestUtils.Logger);

            Assert.AreEqual(userProfile.ReputationBySegment?.GetKeyValueOrNull(segment)?.PointValue, 20);
        }
    }
}
