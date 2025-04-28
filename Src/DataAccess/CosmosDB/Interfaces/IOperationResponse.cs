// <copyright file="IOperationResponse.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace CosmosDB.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos.Scripts;

    public interface IOperationResponse<T>
    {
        public T Item { get; }

        public double RequestUnitsConsumed { get; }

        public bool IsSuccessful { get; }

        public Exception CosmosException { get; }

        Task CaptureResponse(Task<StoredProcedureExecuteResponse<string>> task, T item);
    }
}
