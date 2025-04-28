// <copyright file="EnumParserUtil.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Common.Utils
{
    using System;

    public class EnumParserUtil
    {
        public static T GetEnum<T>(string value)
            where T : struct
        {
            Enum.TryParse(value, true, out T result);

            return result;
        }

        public static string GetString<T>(T value)
            where T : struct
        {
            return Enum.GetName(typeof(T), value);
        }
    }
}
