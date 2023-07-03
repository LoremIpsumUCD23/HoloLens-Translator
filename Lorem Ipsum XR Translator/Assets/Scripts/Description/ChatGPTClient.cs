using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

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

        public IEnumerator SendRequest(Caption caption, Action<Caption> callback)
        {
            // Create body of the request. PromtpRequest -> json -> bytes
            var reqBody = new PromptRequest(this._model, "Description of " + caption.GetPrimaryTitle(), 10, 0.0f);
            byte[] reqBodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(reqBody));

            Caption result = caption;

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
                    result.SetPrimaryDescription("Connection Error: " + www.error);
                    callback(result);
                }
                // Got a reponse with Protocol Error
                else if (www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Protocol Error: " + www.error);
                    result.SetPrimaryDescription("Protocol Error: " + www.error);
                    callback(result);
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
                    result.SetPrimaryDescription(message.Trim(',', '\n'));
                    callback(result);
                }
            }
        }
    }
}
