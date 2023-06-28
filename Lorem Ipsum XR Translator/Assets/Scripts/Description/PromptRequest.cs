using System;

namespace Description
{
    /// <summary>
    /// Request Body
    /// </summary>
    [Serializable]
    class PromptRequest
    {
        public string model;
        public string prompt;
        public int max_tokens;
        public float temperature;

        public PromptRequest(string model, string prompt, int max_tokens, float temperature)
        {
            this.model = model;
            this.prompt = prompt;
            this.max_tokens = max_tokens;
            this.temperature = temperature;
        }
    }
}
