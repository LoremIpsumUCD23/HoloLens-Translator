using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace Description
{
    public class ChatGPTClient : IDescriptionClient
    {
        private const string Url = "https://api.openai.com/v1/completions";
        private readonly string _apiKey;
        private readonly string _model;

        public ChatGPTClient(string apiKey, string model)
        {
            this._apiKey = apiKey;
            this._model = model;
        }

        public IEnumerator SendRequest(string content, Action<string> callback)
        {
            // Create body of the request. PromtpRequest -> json -> bytes
            var reqBody = new PromptRequest(this._model, content, 10, 0.0f);
            byte[] reqBodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(reqBody));

            using (UnityWebRequest www = new UnityWebRequest(ChatGPTClient.Url, UnityWebRequest.kHttpVerbPOST))
            {
                // Set body of the request.
                www.uploadHandler = new UploadHandlerRaw(reqBodyRaw);

                // Set headers
                www.SetRequestHeader("Content-Type", "application/json");
                www.SetRequestHeader("Authorization", "Bearer " + this._apiKey);

                www.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();

                // Send the request and wait for a response
                yield return www.SendWebRequest();

                // Got a response with Connection Error
                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.LogError("Connection Error: " + www.error);
                    callback("Connection Error: " + www.error);
                }
                // Got a reponse with Protocol Error
                else if (www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Protocol Error: " + www.error);
                    callback("Protocol Error: " + www.error);
                }
                // Got a response without any error
                else
                {
                    string message = "";
                    // response text is null
                    if (www.downloadHandler.text == null) message = "downloadHander.text is null.";
                    // response text is an empty string
                    else if (www.downloadHandler.text == "") message = "downloadHander.text is an empty string.";
                    // valid response
                    else
                    {
                        // Parse the response in json into an object.
                        // A response from ChatGPT API should look like what's defined below (Response class).
                        Response res = JsonUtility.FromJson<Response>(www.downloadHandler.text);
                        if (res == null) message = "response is empty.";
                        else if (res.choices == null || res.choices.Count == 0) message = "choices is empty. Read the ChatGPT document for more details.";
                        else message = res.choices[0].text;
                    }
                    Debug.Log(message.Trim(',', '\n'));
                    callback(message.Trim(',', '\n'));
                }
            }
        }

        // Request body.
        [Serializable]
        private class PromptRequest
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

        // Response body
        [Serializable]
        private class Response
        {
            public string id;
            public string object_type;
            public int created;
            public string model;
            public List<Choice> choices;
            public Usage usage;
        }

        [Serializable]
        private class Choice
        {
            public string text;
            public int index;
            public string finish_reason;
        }

        [Serializable]
        private class Usage
        {
            public int prompt_tokens;
            public int completion_tokens;
            public int total_tokens;
        }
    }
}
