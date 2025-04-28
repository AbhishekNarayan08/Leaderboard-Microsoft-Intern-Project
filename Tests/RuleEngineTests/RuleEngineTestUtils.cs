// <copyright file="RuleEngineTestUtils.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace RuleEngineTest
{
    using AzureStorage;
    using CosmosDB.Models;
    using Moq;
    using RuleEngine.Core;
    using RuleEngine.Actions;
    using RuleEngine.Rules;
    using System.Collections.Generic;
    using Common.Models.Enums;
    using Microsoft.Extensions.Logging;

    public static class RuleEngineTestUtils
    {
        static RuleEngineTestUtils()
        {
            Logger = new Mock<ILogger>().Object;

            ActionFactory = new ActionFactory();

            var blobStorageContext = new Mock<IBlobStorageContext>();
            RuleRepository = new LocalRuleRepository(blobStorageContext.Object);

            RuleEngine = new RuleEngine(RuleRepository);
        }

        public static UserProfile UserProfile => new UserProfile
        {
            UserId = "sampleuser123",
            ReputationBySegment = new Dictionary<string, Reputation>
            {
                { SegmentType.Autos.ToString(),
                    new Reputation()
                    {
                        Level = new Level()
                        {
                            CurrentLevel = 1
                        }
                    }
                }
            }
        };

        public static ILogger Logger { get; set; }

        public static IActionFactory ActionFactory { get; set; }

        public static IRuleRepository RuleRepository { get; set; }

        public static IRuleEngine RuleEngine { get; set; }
    }
}
