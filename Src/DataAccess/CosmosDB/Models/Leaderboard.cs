// <copyright file="Leaderboard.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace CosmosDB.Models
{
    public class Leaderboard
    {
        public string LeaderboardId { get; set; }
        public string LeaderboardName { get; set; }

        public string Segment { get; set; }
    }
}