using System;

namespace Description
{
    /// <summary>
    /// A class to hold all the returned information from Merriam Webster's API
    /// </summary>
    [Serializable]
    class Item
    {
        public MetaData meta;
        public HwiData hwi;
        public string fl;
        public DefinitionData[] def;
        public string[] shortdef;
    }
}