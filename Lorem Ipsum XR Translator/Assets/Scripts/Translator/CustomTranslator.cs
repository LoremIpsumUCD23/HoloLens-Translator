using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;


namespace Translator
{
    public class CustomTranslator : ITranslatorClient
    {
        private string host;
        private Dictionary<string, string> models = new Dictionary<string, string>()
        {
            { "en-fr", "opus-mt-en-fr" },
        };

        public CustomTranslator(string host)
        {
            this.host = host;
        }


        public IEnumerator Translate(string originalText, string from, string[] to, Action<string[]> callback)
        {
            if (to.Length != 1) throw new ArgumentException("the number of translated languages has to be 1 for now.");

            string key = from + "-" + to[0];
            if (!models.ContainsKey(key)) throw new ArgumentException(key + " translation is not supported");

            string url = string.Format("{0}/translate?from={1}&to={2}&model={3}", this.host, from, to[0], models[key]);

            // Create the request body
            string requestBody = "{\"text\": \"" + originalText + "\"}";
            byte[] requestData = System.Text.Encoding.UTF8.GetBytes(requestBody);

            // Send a request
            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(requestData);
                request.downloadHandler = new DownloadHandlerBuffer();

                // Set the request header
                request.SetRequestHeader("Content-Type", "application/json");

                // Send the request
                yield return request.SendWebRequest();

                // Handle the response
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log("Error: " + request.error);
                    callback(new string[] { originalText, "Error" });
                }
                else
                {
                    string res = request.downloadHandler.text;
                    TranslationResult decoded = JsonConvert.DeserializeObject<TranslationResult>(res);
                    Debug.Log("Translation response: " + res);
                    callback(new string[] { originalText, decoded.translation });
                }
            }
        }
    }

    [Serializable]
    public class TranslationResult
    {
        public string translation;
    }
}