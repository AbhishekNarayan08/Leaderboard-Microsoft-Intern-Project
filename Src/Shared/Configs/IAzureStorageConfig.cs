// <copyright file="IAzureStorageConfig.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Configs
{
    public interface IAzureStorageConfig
    {
        string ConnectionString { get; }

        int BlobsBatchSize { get; }
    }
}
