// <copyright file="IRuleEngine.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>


namespace RuleEngine.Core
{
    using CosmosDB.Models;
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IRuleEngine
    {
        Task Process(
            UserProfile userProfile,
            ReputationActivity activity,
            ILogger logger);


        Task BatchProcess(
            UserProfile userProfile,
            IEnumerable<ReputationActivity> activities,
            ILogger logger);
    }
}
