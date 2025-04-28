// <copyright file="ReputationByUserIdBatch.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Common.Models.Responses
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class ReputationByUserIdBatch : IBaseResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReputationByUserIdBatch"/> class.
        /// </summary>
        /// <param name="code">ResponseCode</param>
        /// <param name="reputationByUserIdList">List of reputation by user id</param>
        public ReputationByUserIdBatch(ResponseCode code, List<ReputationByUserId> reputationByUserIdList = null)
            : base(code)
        {
            this.ReputationByUserIdList = reputationByUserIdList;
        }

        [JsonProperty(PropertyName = "ReputationByUserIdList")]
        public List<ReputationByUserId> ReputationByUserIdList { get; set; }
    }
}
