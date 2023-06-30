using System;

namespace Description
{
    /// <summary>
    /// Request body to OpenAI API.
    /// </summary>
    [Serializable]
    class PromptRequest
    {
        public string model;
        public string prompt;
        public int max_tokens;
        public float temperature;

        /// <summary> request body </summary>
        /// <see href="https://platform.openai.com/docs/api-reference/completions/create" />
        public PromptRequest(string model, string prompt, int max_tokens, float temperature)
        {
            this.model = model;
            this.prompt = prompt;
            this.max_tokens = max_tokens;
            this.temperature = temperature;
        }
    }
}
