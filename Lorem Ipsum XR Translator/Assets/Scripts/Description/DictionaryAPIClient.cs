using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace Description
{
    /// <summary>
    /// This Dictionary Client uses an API from Merriam Webster Dictionary. The API key comes from a registered account.
    /// The usage limit is 1000 API calls per day for each key.
    /// </summary>
    public class DictionaryAPIClient : IDescriptionClient
    {
        private readonly string _apiKeyElementary = "50975cc1-b4e7-4666-af66-03067dc6060f"; // Elementary dictionary API key
        private readonly string _apiKeyIntermediate = "6009aa88-c0ad-49ef-97fe-c2e785c7d0a8"; // Intermediate dictionary API key
        private readonly string _apiKey;
        private readonly string _model;
        private readonly string _dictionaryRef;



        // model can be either elementary or intermediate
        public DictionaryAPIClient(string model)
        {
            this._model = model;
            if (model.Equals("elementary"))
            {
                this._apiKey = _apiKeyElementary;
                this._dictionaryRef = "sd2";
            }
            else if (model.Equals("intermediate"))
            {
                this._apiKey = _apiKeyIntermediate;
                this._dictionaryRef = "sd3";
            }
            else Debug.LogError("No such model '" + model + "'");
        }

        // returns
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
                        callback(res[0].shortdef[0]);
                        break;
                }
            }
        }

        [Serializable]
        public class Item
        {
            public MetaData meta;
            public HwiData hwi;
            public string fl;
            public DefinitionData[] def;
            public string[] shortdef;
        }

        [Serializable]
        public class MetaData
        {
            public string id;
            public string uuid;
            public string sort;
            public string src;
            public string section;
            public string[] stems;
            public bool offensive;
        }

        [Serializable]
        public class HwiData
        {
            public string hw;
            public PrsData[] prs;
        }

        [Serializable]
        public class PrsData
        {
            public string mw;
            public SoundData sound;
        }

        [Serializable]
        public class SoundData
        {
            public string audio;
        }

        [Serializable]
        public class DefinitionData
        {
            public object[][] sseq;
        }
    }
}
