// <copyright file="IRuleRepository.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace RuleEngine.Rules
{
    using Common.Models.Enums;
    using CosmosDB.Models;
    using RulesEngine.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IRuleRepository
    {
        Task<List<Workflow>?> GetRulesForActivity(
            ReputationActivity activity,
            bool usecache = false);

        Task<List<Workflow>?> GetLatestRulesForSegment(
            SegmentType segment,
            bool usecache = false);
    }
}
