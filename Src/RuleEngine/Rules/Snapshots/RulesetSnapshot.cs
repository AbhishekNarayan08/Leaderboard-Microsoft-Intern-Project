// <copyright file="RuleSnapshot.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace RuleEngine.Rules.Snapshots
{
    using Newtonsoft.Json;
    using RulesEngine.Models;
    using System.Collections.Generic;

    public class RulesetSnapshot
    {
        [JsonProperty(PropertyName = "metadata")]
        public SnapshotMetadata Metadata { get; set; }

        [JsonRequired]
        [JsonProperty(PropertyName = "workflows")]
        public List<Workflow> Workflows { get; set; }
    }
}
