// <copyright file="ValueUtils.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Common.Utils
{
    using System;
    using System.Collections.Generic;

    public static class ValueUtils
    {
        public static TVal? GetKeyValueOrNull<TKey, TVal>(
            this IDictionary<TKey, TVal>? dict,
            TKey key,
            string? paramName = null,
            bool throwIfDictNull = false)
            where TKey : notnull
            where TVal : class
        {
            if (dict == null)
            {
                if (throwIfDictNull)
                {
                    if (!string.IsNullOrWhiteSpace(paramName))
                    {
                        throw new ArgumentNullException(nameof(dict));
                    }
                    else
                    {
                        throw new ArgumentNullException(paramName);
                    }
                }

                return null;
            }

            if (!dict.TryGetValue(key, out var val))
            {
                return null;
            }

            return val;
        }
    }
}
