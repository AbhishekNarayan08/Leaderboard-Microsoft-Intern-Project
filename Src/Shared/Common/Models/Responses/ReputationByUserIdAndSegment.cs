// <copyright file="ReputationByUserIdAndSegment.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Common.Models.Responses
{
    using Newtonsoft.Json;

    public class ReputationByUserIdAndSegment : IBaseResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReputationByUserIdAndSegment"/> class.
        /// </summary>
        /// <param name="code">ResponseCode</param>
        /// <param name="reputation">Reputation</param>
        public ReputationByUserIdAndSegment(ResponseCode code, Reputation reputation = null)
            : base(code)
        {
            this.Reputation = reputation;
        }

        [JsonProperty(PropertyName = "Reputation")]
        public Reputation Reputation { get; set; }
    }
}
