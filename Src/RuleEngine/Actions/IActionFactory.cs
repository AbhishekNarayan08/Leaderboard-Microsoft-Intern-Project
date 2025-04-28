// <copyright file="IActionFactory.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace RuleEngine.Actions
{
    using RulesEngine.Models;

    public interface IActionFactory
    {
        IAction? DetermineAction(RuleResultTree rule);
    }
}
