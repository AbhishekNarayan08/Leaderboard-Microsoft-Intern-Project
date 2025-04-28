// <copyright file="JsonConvertUtil.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Common.Utils
{
    using System.Collections.Generic;
    using Common.Models.Requests;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class JsonConvertUtil
    {
        static JsonConvertUtil()
        {
            SerializationSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.None,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter>()
                {
                    new StringEnumConverter(),
                },
            };
        }

        public static JsonSerializerSettings SerializationSettings { get; private set; }

        public static ActivityRequest DeserializeActivityRequest(string requestBody)
        {
            if (string.IsNullOrWhiteSpace(requestBody))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<ActivityRequest>(requestBody);
        }

        public static string SerializeActivityRequest(ActivityRequest activityRequest)
        {
            if (activityRequest == null)
            {
                return null;
            }

            return JsonConvert.SerializeObject(activityRequest);
        }

        public static string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value, SerializationSettings);
        }

        public static T DeserializeObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, SerializationSettings);
        }
    }
}
