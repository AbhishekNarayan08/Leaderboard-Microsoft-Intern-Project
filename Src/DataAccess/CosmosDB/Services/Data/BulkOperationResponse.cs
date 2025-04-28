// <copyright file="BulkOperationResponse.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace CosmosDB.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class BulkOperationResponse<T>
    {
        public int SuccessfulDocuments { get; set; } = 0;

        public double TotalRequestUnitsConsumed { get; set; } = 0;

        public IReadOnlyList<(T, Exception)> Failures { get; set; }
    }
}
