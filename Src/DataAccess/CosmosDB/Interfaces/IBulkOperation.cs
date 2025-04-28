// <copyright file="IBulkOperation.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace CosmosDB.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using CosmosDB.Services.Data;

    public interface IBulkOperation<T>
    {
        Task<BulkOperationResponse<T>> ExecuteAsync(List<dynamic> items, string partitionKey);
    }
}
