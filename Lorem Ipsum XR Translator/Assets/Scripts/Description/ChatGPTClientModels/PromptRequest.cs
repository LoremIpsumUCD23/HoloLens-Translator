using System;
using System.Collections.Generic;

namespace Description
{
    /// <summary>
    /// Request body to OpenAI API.
    /// </summary>
    [Serializable]
    class PromptRequest
    {
        public string model;
        public List<Message> messages;

        /// <summary> request body </summary>
        /// <see href=https://platform.openai.com/docs/api-reference/chat/create />
        public PromptRequest(string model, List<Message> messages)
        {
            this.model = model;
            this.messages = messages;
        }
    }

    [Serializable]
    class Message
    {
        public string role;
        public string content;
        public Message(string role, string content)
        {
            this.role = role;
            this.content = content;
        }
    }
}
