using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace Description
{
    /// <summary>
    /// This Dictionary Client uses an API from Merriam-Webster Dictionary. 
    /// The usage limit is 1000 API calls per day for each key.
    /// </summary>
    public class DictionaryAPIClient : IDescriptionClient
    {
        private readonly string _apiKey;
        private readonly string _dictionaryRef;

        /// <summary>
        /// The constructor for the Dictionary API client
        /// It has two possible models to choose from, 'elementary' or 'intermediate'
        /// </summary>
        /// <param name="model"> model can be either 'elementary' or 'intermediate' for now</param>
        /// <param name="apiKey"> API key</param>

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
        /// <summary>
        /// This method implements an interface forced by IDescriptionClient. It sends a request to MerriamWebster api and
        /// passes the response from the api to <paramref name="callback"/> at the end. If it causes an error at
        /// some point of this procedure, it passes the error message to <paramref name="callback"/>.
        /// </summary>
        /// <param name="content">Word to be sent to the API</param>
        /// <param name="callback">An action that gets executed with the translated text</param>
        /// <returns></returns>
        public IEnumerator Explain(string content, Action<string[]> callback)
        {
            // URI for HTTP Calls
            string uri = "https://dictionaryapi.com/api/v3/references/" + this._dictionaryRef + "/json/" + content + "?key=" + this._apiKey;

            string[] returnString = new string[2];
            returnString[0] = content;

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
                        returnString[1] = "Connection Error: " + webRequest.error;
                        Debug.LogError(returnString[1]);
                        callback(returnString);
                        break;
                    // Data Processing error
                    case UnityWebRequest.Result.DataProcessingError:
                        returnString[1] = "Dataprocessing Error: " + webRequest.error;
                        Debug.LogError(returnString[1]);
                        callback(returnString);
                        break;
                    // HTTP error
                    case UnityWebRequest.Result.ProtocolError:
                        returnString[1] = "Http Error: " + webRequest.error;
                        Debug.LogError(returnString[1]);
                        callback(returnString);
                        break;
                    // No Error
                    case UnityWebRequest.Result.Success:
                        Debug.Log(webRequest.downloadHandler.text);
                        List<Item> res = JsonConvert.DeserializeObject<List<Item>>(webRequest.downloadHandler.text);
                        if (res.Count == 0){
                            returnString[1] = "No such word";
                            callback(returnString);
                            break;
                        }
                        returnString[1] = res[0].shortdef[0];
                        callback(returnString);
                        break;
                }
            }
        }

        // Documentation for JSON classes is at: https://dictionaryapi.com/products/json
    }
}
