using System;

namespace Description
{
    /// <summary>
    /// Definition Information
    /// This one contains sense sequences for a headword.
    /// Senses for a word is a unit of the headword that gathers all content relevant to a particular meaning that headword.
    /// Basically the various meanings of a word.
    /// </summary>
    [Serializable]
    public class DefinitionData
    {
        public object[][] sseq;
    }
}