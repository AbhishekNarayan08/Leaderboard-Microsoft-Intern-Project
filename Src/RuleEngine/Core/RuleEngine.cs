// <copyright file="RuleEngine.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace RuleEngine.Core
{
    using Common.Models.Enums;
    using Common.Utils;
    using CosmosDB.Models;
    using global::RuleEngine.Actions;
    using global::RuleEngine.Rules;
    using Microsoft.Extensions.Logging;
    using RulesEngine;
    using RulesEngine.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class RuleEngine : IRuleEngine
    {
        // TODO adlohana: move to config
        private const int MaxPasses = 10;

        protected IRuleRepository RuleRepository { get; set; }

        protected IActionFactory ActionFactory { get; set; }

        public RuleEngine(IRuleRepository ruleRepository)
        {
            this.RuleRepository = ruleRepository;
            this.ActionFactory = new ActionFactory();
        }


        public async Task BatchProcess(
            UserProfile userProfile, 
            IEnumerable<ReputationActivity> activities,
            ILogger logger)
        {
            if (activities == null)
            {
                return;
            }

            foreach (var activity in activities)
            {
                await this.Process(
                    userProfile: userProfile,
                    activity: activity,
                    logger: logger);
            }

            return;
        }

        public async Task Process(
            UserProfile userProfile,
            ReputationActivity activity,
            ILogger logger)
        {
            var segment = EnumParserUtil.GetEnum<SegmentType>(activity.Segment);

            if (segment == SegmentType.Unknown)
            {
                logger.LogError($"Unknown activity type encountered for activity id {activity.Id}");
                return;
            }

            var ruleSet = await this.RuleRepository.GetRulesForActivity(activity);

            if (ruleSet?.Count > 0)
            {
                // To support triggering rules that might succeed after the action for another rule being applied,
                // we need to go over the rule-set multiple times. An unfired rule will succeed IFF an action was applied
                // during the last pass. Our rule-set shrinks every pass to include rules that did not fire in the last pass.
                // NOTE: We have a ceiling on the number of passes to avoid getting stuck in cascading rule fires.
                int pass = 0;

                while (pass < MaxPasses
                    && ruleSet?.Any(wf => wf.Rules != null && wf.Rules.Count() > 0) == true)
                {
                    ruleSet = await this.Process(userProfile, activity, ruleSet, segment);
                    pass++;
                }

                this.RecordActivity(userProfile, activity, segment);
                logger.LogInformation($"Executed engine for user id: {userProfile.Id}, activity id: {activity.Id} and segment: {segment} in {pass} pass(es)");
            }
            else 
            {
                logger.LogInformation($"Rules could not be found for segment {segment}. Skipping activity with id: {activity.Id}");
            }

            return;
        }

        private async Task<List<Workflow>> Process(
            UserProfile userProfile,
            ReputationActivity activity, 
            List<Workflow> ruleset, 
            SegmentType segment)
        {
            var ruleParams = new RuleParameter[]
            {
                    new RuleParameter(nameof(UserProfile), userProfile),
                    new RuleParameter(nameof(ReputationActivity), activity),
            };

            var failedRules = new List<Rule>();

            var engine = new RulesEngine(ruleset.ToArray());
            var results = await engine.ExecuteAllRulesAsync(segment.ToString(), ruleParams);

            foreach (var result in results)
            {
                if (result.IsSuccess)
                {
                    var action = this.ActionFactory.DetermineAction(result);
                    if (action != null)
                    {
                        action.Execute(userProfile, activity, segment, result);
                    }
                }
                else
                {
                    failedRules.Add(result.Rule);
                }
            }

            var outputWorkflows = new List<Workflow>()
            {
                new Workflow()
                {
                    WorkflowName = segment.ToString(),
                    // If no rule was fired during the last pass, we can stop processing the inputs.
                    Rules = results.Any(r => r.IsSuccess)
                    ? failedRules
                    : new List<Rule>(),
                }
            };

            return outputWorkflows;
        }

        // TODO adlohana: refactor dictionary access and updates
        private void RecordActivity(
            UserProfile userProfile,
            ReputationActivity activity,
            SegmentType segment)
        {
            var reputationDict = userProfile.ReputationBySegment
                ?? new Dictionary<string, Reputation>();
            var segmentReputation = reputationDict.GetKeyValueOrNull(segment.ToString())
                ?? new Reputation();
            var activityStats = segmentReputation.ActivityStats
                ?? new Dictionary<string, ActivityStats>();

            var activityType = EnumParserUtil.GetEnum<ActivityType>(activity.ActivityType);
            if (activityType == ActivityType.Unknown)
            {
                throw new Exception("Insufficient info available to record activity");
            }
            else
            {
                var activityTypeString = activityType.ToString();
                var stats = activityStats.GetKeyValueOrNull(activityTypeString)
                    ?? new ActivityStats();
                stats.CountValue += 1;
                activityStats[activityTypeString] = stats;
                segmentReputation.ActivityStats = activityStats;
                segmentReputation.LastUpdated = DateTime.UtcNow;
                reputationDict[segment.ToString()] = segmentReputation;
                userProfile.ReputationBySegment = reputationDict;
            }
        }
    }
}
