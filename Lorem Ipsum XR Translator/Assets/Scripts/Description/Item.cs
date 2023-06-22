using System;

namespace Description
{
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