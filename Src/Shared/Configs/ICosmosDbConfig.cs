// <copyright file="ICosmosDbConfig.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Configs
{
    public interface ICosmosDbConfig
    {
        string EndPointUrl { get; }

        string PrimaryKey { get; }
    }
}
