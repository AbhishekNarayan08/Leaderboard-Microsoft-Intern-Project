// <copyright file="LocalRuleRepository.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace RuleEngineTest
{
    using AzureStorage;
    using Common.Models.Enums;
    using Common.Utils;
    using RuleEngine.Rules;
    using RuleEngine.Rules.Snapshots;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    public class LocalRuleRepository : RuleRepository
    {
        public LocalRuleRepository(IBlobStorageContext blobStorageContext)
            : base(blobStorageContext)
        {
        }

        protected override async Task<List<RulesetSnapshot>> FetchSegmentRulesetSnapshots(SegmentType segmentType)
        {
            var rulesetSnasphsots = new List<RulesetSnapshot>();

            var currentDirectory = Directory.GetCurrentDirectory();
            var rulesFiles = Directory.GetFiles($"{currentDirectory}\\Files\\{segmentType}");

            foreach (var file in rulesFiles)
            {
                if (!File.Exists(file))
                {
                    continue;
                }

                var ruleSetSnapshotText = File.ReadAllText(file);
                var rulesetSnasphsot = JsonConvertUtil.DeserializeObject<RulesetSnapshot>(ruleSetSnapshotText);

                if (rulesetSnasphsot != null)
                {
                    rulesetSnasphsots.Add(rulesetSnasphsot);
                }
            }

            return await Task.FromResult(rulesetSnasphsots);
        }
    }
}
