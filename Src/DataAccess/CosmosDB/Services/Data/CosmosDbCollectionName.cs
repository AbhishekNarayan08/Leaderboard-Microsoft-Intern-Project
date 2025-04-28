// <copyright file="CosmosDbCollectionName.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace CosmosDB.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public enum CosmosDbCollectionName
    {
        Users = 1,
        Leaderboards = 2,
        UserLeaderboards = 3,
        LeaderboardUser = 4,
    }
}
