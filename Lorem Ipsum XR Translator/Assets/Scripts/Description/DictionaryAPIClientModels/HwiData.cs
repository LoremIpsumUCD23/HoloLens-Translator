using System;

namespace Description
{
    /// <summary>
    /// Headword Information
    /// It is the word being defined
    /// </summary>
    [Serializable]
    public class HwiData
    {
        public string hw;
        public PrsData[] prs;
    }
}