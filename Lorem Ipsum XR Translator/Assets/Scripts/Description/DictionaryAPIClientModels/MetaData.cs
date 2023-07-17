using System;

namespace Description
{
    /// <summary>
    /// Metadata for the word
    /// </summary>
    [Serializable]
    public class MetaData
    {
        public string id;
        public string uuid;
        public string sort;
        public string src;
        public string section;
        public string[] stems;
        public bool offensive;
    }
}