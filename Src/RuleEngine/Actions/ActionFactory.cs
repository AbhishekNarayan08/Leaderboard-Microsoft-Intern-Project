// <copyright file="ActionFactory.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace RuleEngine.Actions
{
    using RulesEngine.Models;
    using System;

    public class ActionFactory : IActionFactory
    {
        public IAction? DetermineAction(RuleResultTree rule)
        {
            var rawActionType = rule.Inputs["Action"]?.ToString();
            if (Enum.TryParse<ActionType>(rawActionType, out var actionType))
            {
                return actionType switch
                {
                    ActionType.IncrementReputation => new IncrementReputationAction(),
                    ActionType.AddBage => new BadgeAdditionAction(),
                    ActionType.UpdateLevel => new UpdateLevelAction(),
                    _ => throw new Exception("Invalid action")
                };
            }

            throw new Exception("Action not found for rule");
        }
    }
}
