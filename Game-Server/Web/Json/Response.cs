using System;
using Newtonsoft.Json;

namespace Game_Server.Util
{
    public class Response
    {
        /// <summary>
        /// Status code
        /// </summary>
        [JsonProperty("success")]
        public int Success;

        [JsonProperty("reponseMsg")]
        public string Data;
    }
}
