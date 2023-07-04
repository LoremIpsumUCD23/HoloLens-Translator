using System;

namespace Description
{
    /// <summary>
    /// Choices among which we can choose description from returned by OpenAI api.
    /// </summary>
    [Serializable]
    class Choice
    {
        public string text;
        public int index;
        public string finish_reason;
    }
}
