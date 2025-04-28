// <copyright file="IBaseResponse.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Common.Models.Responses
{
    using Newtonsoft.Json;

    public class IBaseResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IBaseResponse"/> class.
        /// </summary>
        /// <param name="code">Response code</param>
        public IBaseResponse(ResponseCode code)
        {
            this.Code = code;
            this.Message = this.GetMessage(code);
        }

        [JsonProperty(PropertyName = "Code")]
        public ResponseCode Code { get; set; }

        [JsonProperty(PropertyName = "Message")]
        public string Message { get; set; }

        private string GetMessage(ResponseCode code)
        {
            switch (code)
            {
                case ResponseCode.SUCCESS:
                    return "Success";
                case ResponseCode.INVALID_ACTIVITY_ID:
                    return "Invalid activity id, please pass non empty active activity id.";
                case ResponseCode.ACTIVITY_ID_ALREADY_EXISTS:
                    return "User activity id already exists.";
                case ResponseCode.INVALID_SEGMENT_ID:
                    return "Invalid segment id, please pass non empty active segment id.";
                default:
                    return code.ToString();
            }
        }
    }
}
