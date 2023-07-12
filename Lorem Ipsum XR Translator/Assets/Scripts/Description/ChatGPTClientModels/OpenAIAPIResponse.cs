using System;
using System.Collections.Generic;

namespace Description
{
    /// <summary>
    /// Response body used in the Explain method of ChatGPTClient.
    /// </summary>
    [Serializable]
    class OpenAIAPIResponse
    {
        public string id;
        public string object_type;
        public int created;
        public string model;
        public List<Choice> choices;
        public Usage usage;
    }
}
