// <copyright file="Activity.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Common.Models.Responses
{
    using Common.Models.Enums;
    using Newtonsoft.Json;

    public class Activity
    {
        [JsonProperty(PropertyName = "ActivityId")]
        public string ActivityId { get; set; }

        [JsonProperty(PropertyName = "UserId")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "ActivityType")]
        public ActivityType ActivityType { get; set; }

        [JsonProperty(PropertyName = "ActivityTypeString")]
        public string ActivityTypeString { get; set; }

        [JsonProperty(PropertyName = "SegmentType")]
        public SegmentType SegmentType { get; set; }

        [JsonProperty(PropertyName = "SegmentTypeString")]
        public string SegmentTypeString { get; set; }

        [JsonProperty(PropertyName = "ActivityPoints")]
        public long ActivityPoints { get; set; }

        [JsonProperty(PropertyName = "Content")]
        public ContentMetadata Content { get; set; }
    }
}
