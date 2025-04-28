// <copyright file="Badge.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace CosmosDB.Models
{
    using System;
    using Newtonsoft.Json;

    public class Badge
    {
        [JsonProperty(PropertyName = "badgeId")]
        public string BadgeId { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "creationDate")]
        public DateTimeOffset CreationDate { get; set; }
    }
}
