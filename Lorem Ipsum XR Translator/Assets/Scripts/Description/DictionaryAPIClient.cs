using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace Description
{
    /// <summary>
    /// This Dictionary Client uses an API from Merriam-Webster Dictionary. The API key comes from a registered account.
    /// The usage limit is 1000 API calls per day for each key.
    /// </summary>
    public class DictionaryAPIClient : IDescriptionClient
    {
        private readonly string _apiKey;
        private readonly string _dictionaryRef;

        // model can be either elementary or intermediate
        public DictionaryAPIClient(string model, string apiKey)
        {
            this._apiKey = apiKey;
            if (model.Equals("elementary"))
            {
                this._dictionaryRef = "sd2";
            }
            else if (model.Equals("intermediate"))
            {
                this._dictionaryRef = "sd3";
            }
            else Debug.LogError("No such model '" + model + "'");
        }

        // returns the dictionary description of a word
        public IEnumerator SendRequest(string content, Action<string> callback)
        {
            // URI for HTTP Calls
            string uri = "https://dictionaryapi.com/api/v3/references/" + this._dictionaryRef + "/json/" + content + "?key=" + this._apiKey;

            // Get call
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                // Switch-case for errors
                switch (webRequest.result)
                {
                    // Connection error
                    case UnityWebRequest.Result.ConnectionError:
                        Debug.LogError("Connection Error: " + webRequest.error);
                        callback("Connection Error: " + webRequest.error);
                        break;
                    // Data Processing error
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError("Dataprocessing Error: " + webRequest.error);
                        callback("Dataprocessing Error: " + webRequest.error);
                        break;
                    // HTTP error
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError("Http Error: " + webRequest.error);
                        callback("Http Error: " + webRequest.error);
                        break;
                    // No Error
                    case UnityWebRequest.Result.Success:
                        List<Item> res = JsonConvert.DeserializeObject<List<Item>>(webRequest.downloadHandler.text);
                        if (res.Count == 0){
                            callback("No such word");
                            break;
                        }
                        callback(res[0].shortdef[0]);
                        break;
                }
            }
        }

        // Documentation for JSON classes is at: https://dictionaryapi.com/products/json
    }
}
