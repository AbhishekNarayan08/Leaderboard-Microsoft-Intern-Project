// <copyright file="AzureStorageConfig.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Configs
{
    public class AzureStorageConfig : IAzureStorageConfig
    {
        public string ConnectionString { get; private set; } = ConfigUtils.RetrieveVerifyConfigValue("BlobStorageConnectionString");

        public int BlobsBatchSize { get; private set; } = ConfigUtils.RetrieveVerifyNumberConfigValue("BlobsBatchSize");
    }
}
