// <copyright file="IAction.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace RuleEngine.Actions
{
    using Common.Models.Enums;
    using CosmosDB.Models;
    using RulesEngine.Models;

    public interface IAction
    {
        void Execute(
            UserProfile userProfile,
            ReputationActivity activity,
            SegmentType segment,
            RuleResultTree ruleResult);
    }
}
