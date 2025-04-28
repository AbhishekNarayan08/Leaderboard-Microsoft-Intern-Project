// <copyright file="ReputationByUserIdAndSegmentBatch.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Common.Models.Responses
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class ReputationByUserIdAndSegmentBatch : IBaseResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReputationByUserIdAndSegmentBatch"/> class.
        /// </summary>
        /// <param name="code">ResponseCode</param>
        /// <param name="reputationByUserIdAndSegment">Reputation by user id and batch</param>
        public ReputationByUserIdAndSegmentBatch(ResponseCode code, List<ReputationByUserIdAndSegment> reputationByUserIdAndSegment = null)
            : base(code)
        {
            this.ReputationByUserIdAndSegment = reputationByUserIdAndSegment;
        }

        [JsonProperty(PropertyName = "ReputationByUserIdAndSegment")]
        public List<ReputationByUserIdAndSegment> ReputationByUserIdAndSegment { get; set; }
    }
}
