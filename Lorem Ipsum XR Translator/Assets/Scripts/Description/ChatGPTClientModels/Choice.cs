using System;

namespace Description
{
    /// <summary>
    /// Choices among which we can choose description from returned by OpenAI api.
    /// </summary>
    [Serializable]
    class Choice
    {
        public int index;
        public Message message;
        public string finish_reason;
    }
}
