// <copyright file="SnapshotMetadata.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace RuleEngine.Rules.Snapshots
{
    using Common.Models.Enums;
    using Newtonsoft.Json;
    using System;

    public class SnapshotMetadata
    {
        [JsonRequired]
        [JsonProperty(PropertyName = "segment")]
        public SegmentType Segment { get; set; }

        [JsonRequired]
        [JsonProperty(PropertyName = "version")]
        public uint Version { get; set; }

        [JsonRequired]
        [JsonProperty(PropertyName = "creationDateTime")]
        public DateTime CreationDateTime { get; set; }
    }
}
