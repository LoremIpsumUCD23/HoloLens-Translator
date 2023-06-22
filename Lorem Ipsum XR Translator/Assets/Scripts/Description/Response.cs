using System;
using System.Collections.Generic;

namespace Description
{
    /// <summary>
    /// Response Body
    /// </summary>
    [Serializable]
     class Response
    {
        public string id;
        public string object_type;
        public int created;
        public string model;
        public List<Choice> choices;
        public Usage usage;
    }
}
