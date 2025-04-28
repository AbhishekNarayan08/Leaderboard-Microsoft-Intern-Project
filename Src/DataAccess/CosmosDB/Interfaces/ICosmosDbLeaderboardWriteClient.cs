// <copyright file="ICosmosDbLeaderboardWriteClient.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace CosmosDB.Interfaces
{
    using System.Threading.Tasks;

    public interface ICosmosDbLeaderboardWriteClient
    {
        Task UpsertDocumentAsync<T>(
             string partitionKeyPath,
             T document);
    }
}
