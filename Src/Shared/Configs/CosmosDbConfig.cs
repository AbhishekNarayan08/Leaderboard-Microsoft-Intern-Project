// <copyright file="CosmosDbConfig.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Configs
{
    public class CosmosDbConfig : ICosmosDbConfig
    {
        //public string EndPointUrl { get; private set; } = ConfigUtils.RetrieveVerifyConfigValue("CosmosDbEndPointUrl");

        //public string PrimaryKey { get; private set; } = ConfigUtils.RetrieveVerifyConfigValue("CosmosDbPrimaryKey");

        public string? EndPointUrl { get; set; }
        public string? PrimaryKey { get; set; }
    }
}
