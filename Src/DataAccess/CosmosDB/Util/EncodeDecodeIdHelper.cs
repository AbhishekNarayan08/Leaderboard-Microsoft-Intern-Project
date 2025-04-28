// <copyright file="EncodeDecodeIdHelper.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace CosmosDB.Util
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class EncodeDecodeIdHelper
    {
        /// <summary>
        /// Encodes the specified plain text.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <returns>encoded text.</returns>
        public static string Encode(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return null;
            }

            return Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes(plainText))
                /* Base64 has '/' which is not allowed in cosmos db id field.
                 So, Replace it with non-base64 valid character */
                .Replace("/", "-");
        }

        /// <summary>
        /// Decodes the specified encoded text.
        /// </summary>
        /// <param name="encodedText">The encoded text.</param>
        /// <returns>decoded text./returns>
        public static string Decode(string encodedText)
        {
            if (string.IsNullOrEmpty(encodedText))
            {
                return null;
            }

            /* replace hyphen with / to make it valid base64v encoded string and
             * then decode it*/
            return Encoding.UTF8.GetString(Convert.FromBase64String(
                encodedText.Replace("-", "/")));
        }
    }
}
