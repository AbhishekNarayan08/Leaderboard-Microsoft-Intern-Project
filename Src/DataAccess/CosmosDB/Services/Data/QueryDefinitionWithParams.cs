// <copyright file="QueryDefinitionWithParams.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace CosmosDB.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Common.Utils;
    using Microsoft.Azure.Cosmos;

    public class QueryDefinitionWithParams : QueryDefinition
    {
        private Dictionary<string, object> parameters;

        public QueryDefinitionWithParams(string query)
            : base(query)
        {
            this.parameters = new Dictionary<string, object>();
        }

        public new QueryDefinitionWithParams WithParameter(string name, object value)
        {
            if (this.parameters.ContainsKey(name))
            {
                this.parameters[name] = value;
            }
            else
            {
                this.parameters.Add(name, value);
            }

            base.WithParameter(name, value);
            return this;
        }

        public override string ToString()
        {
            var strBuilder = new StringBuilder();
            strBuilder.Append(this.QueryText);

            if (this.parameters.Any())
            {
                strBuilder.Append("\r\nParameters: ");

                foreach (var parameter in this.parameters)
                {
                    strBuilder.Append($"{parameter.Key}={JsonConvertUtil.SerializeObject(parameter.Value)}; ");
                }
            }

            return strBuilder.ToString();
        }
    }
}
