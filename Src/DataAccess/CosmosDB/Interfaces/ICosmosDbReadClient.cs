// <copyright file="ICosmosDbReadClient.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace CosmosDB.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CosmosDB.Services.Data;
    using Microsoft.Azure.Cosmos;

    public interface ICosmosDbReadClient
    {

        IAsyncEnumerable<T> QueryAsync<T>(
            QueryDefinitionWithParams query,
            string partitionKey);

        Task<FeedResponse<T>> QuerySinglePageAsync<T>(
            QueryDefinitionWithParams query,
            string partitionKey);
    }
}
