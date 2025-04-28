// <copyright file="OperationResponse.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace CosmosDB.Services.Data
{
    using CosmosDB.Interfaces;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Azure.Cosmos.Scripts;
    using System;
    using System.Threading.Tasks;

    public class OperationResponse<T> : IOperationResponse<T>
    {
        private T item;
        private double requestUnitsConsumed;
        private bool isSuccessful;
        private Exception cosmosException;
        Task<StoredProcedureExecuteResponse<string>> task;

        public async Task CaptureResponse(Task<StoredProcedureExecuteResponse<string>> task, T item)
        {
            this.item = item;

            try
            {
                StoredProcedureExecuteResponse<string> response = await task;

                this.isSuccessful = true;
                this.requestUnitsConsumed = task.Result.RequestCharge;
            }
            catch (Exception ex)
            {
                if (ex is CosmosException cosmosException)
                {
                    this.isSuccessful = false;
                    this.requestUnitsConsumed = task.Result.RequestCharge;
                    this.cosmosException = cosmosException;
                }
                else
                {
                    this.isSuccessful = true;
                    this.cosmosException = ex;
                }
            }
        }

        public T Item { get => this.item; }
        public double RequestUnitsConsumed { get => this.requestUnitsConsumed; }
        public bool IsSuccessful { get => this.isSuccessful; }
        public Exception CosmosException { get => this.cosmosException; }
    }
}
