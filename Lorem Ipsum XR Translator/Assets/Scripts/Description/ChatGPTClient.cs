using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Description
{
    /// <summary>
    /// Class ChatGPTClient utilises OpenAI api to describe things.
    /// </summary>
    public class ChatGPTClient : IDescriptionClient
    {
        private const string Url = "https://api.openai.com/v1/completions";
        private readonly string _apiKey;
        private readonly string _model;

        /// <summary>The constructor of ChatGPTClient. It takes an api key for connection and ChatGPT model.</summary>
        /// <see href="https://platform.openai.com/account/api-keys" />
        /// <see href="https://platform.openai.com/docs/api-reference/models" />
        public ChatGPTClient(string apiKey, string model)
        {
            this._apiKey = apiKey;
            this._model = model;
        }

        /// <summary>
        /// This method implements an interface forced by IDescriptionClient. It sends a request to OpenAI api and
        /// passes the response from the api to <paramref name="callback"/> at the end. If it causes an error at
        /// some point of this procedure, it passes the error message to <paramref name="callback"/>.
        /// </summary>
        /// <param name="content">Text that is explained in this method.</param>
        /// <param name="callback">An action that gets executed with the translated text.</param>
        public IEnumerator Explain(string content, Action<string[]> callback)
        {
            // Create body of the request. PromtpRequest -> json -> bytes
            var reqBody = new PromptRequest(this._model, content, 10, 0.0f);
            byte[] reqBodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(reqBody));

            string[] returnString = new string[2];
            returnString[0] = content;

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
                    returnString[1] = "Connection Error: " + www.error;
                    Debug.LogError(returnString[1]);
                    callback(returnString);
                }
                // Got a reponse with Protocol Error
                else if (www.result == UnityWebRequest.Result.ProtocolError)
                {
                    returnString[1] = "Protocol Error: " + www.error;
                    Debug.LogError(returnString[1]);
                    callback(returnString);
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
                        OpenAIAPIResponse res = JsonUtility.FromJson<OpenAIAPIResponse>(www.downloadHandler.text);
                        if (res == null) message = "response is empty.";
                        else if (res.choices == null || res.choices.Count == 0) message = "choices is empty. Read the ChatGPT document for more details.";
                        else message = res.choices[0].text;
                    }
                    returnString[1] = message.Trim(',', '\n');
                    Debug.Log(returnString[1]);
                    callback(returnString);
                }
            }
        }
    }
}
