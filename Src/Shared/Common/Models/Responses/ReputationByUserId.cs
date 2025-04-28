// <copyright file="ReputationByUserId.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Common.Models.Responses
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class ReputationByUserId : IBaseResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReputationByUserId"/> class.
        /// </summary>
        /// <param name="code">ResponseCode</param>
        /// <param name="reputation">Reputation data</param>
        /// <param name="reputationBySegment">Reputation data by segment</param>
        public ReputationByUserId(ResponseCode code, Reputation reputation = null, List<Reputation> reputationBySegment = null)
            : base(code)
        {
            this.Reputation = reputation;
            this.ReputationBySegment = reputationBySegment;
        }

        [JsonProperty(PropertyName = "Reputation")]
        public Reputation Reputation { get; set; }

        [JsonProperty(PropertyName = "ReputationBySegment")]
        public List<Reputation> ReputationBySegment { get; set; }
    }
}
