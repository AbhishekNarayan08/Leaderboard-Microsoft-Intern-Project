// <copyright file="ActivityFeedByUserId.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Common.Models.Responses
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class ActivityFeedByUserId : IBaseResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityFeedByUserId"/> class.
        /// </summary>
        /// <param name="code">ResponseCode</param>
        /// <param name="activityFeed">Acitivity feed list</param>
        public ActivityFeedByUserId(ResponseCode code, List<Activity> activityFeed = null)
            : base(code)
        {
            this.ActivityFeed = activityFeed;
        }

        [JsonProperty(PropertyName = "ActivityFeed")]
        public List<Activity> ActivityFeed { get; set; }
    }
}
