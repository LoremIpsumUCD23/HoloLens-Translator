using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Net;


namespace Translator
{

    public class AzureTranslator : ITranslatorClient
    {
        private const string endpoint = "https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&";

        // location, also known as region.
        // required if you're using a multi-service or regional (not global) resource. It can be found in the Azure portal on the Keys and Endpoint page.
        private readonly string apiKey;
        private readonly string location;

        public AzureTranslator(string apiKey,string location)
        {
            this.apiKey = apiKey;
            this.location = location;
        }


        public IEnumerator Translate(Caption caption, string from, string[] to, Action<Caption> callback)
        {
            string url = AzureTranslator.endpoint + string.Format("from={0}", from);
            for (int i = 0; i < to.Length; i ++)
            {
                url += string.Format("&to={0}", to[i]);
            }

            // Create the request body
            string requestBody = "[{ \"Text\": \"" + caption.GetPrimaryTitle() + "<|>" + caption.GetPrimaryDescription() + "\" }]";

            // Send a request
            using (UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
            {
                byte[] requestData = System.Text.Encoding.UTF8.GetBytes(requestBody);
                request.uploadHandler = new UploadHandlerRaw(requestData);

                // Set the request headers
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Ocp-Apim-Subscription-Key", this.apiKey);
                request.SetRequestHeader("Ocp-Apim-Subscription-Region", this.location);

                request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();

                // Send the request
                yield return request.SendWebRequest();

                // Request sent, clean up our request data to prevent a memory leak.
                // TODO: See if there's a better way to handle this. This seems like it could be expensive.
                requestData = null;
                GC.Collect();

                // Handle the response
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log("Error: " + request.error);
                    caption.SetTranslatedDescription("Error: " + request.error);
                    callback(caption);
                }
                else
                {
                    string responseText = request.downloadHandler.text;
                    List<Translations> res = JsonConvert.DeserializeObject<List<Translations>>(responseText);
                    // Parse and process the response as needed
                    Debug.Log("Translation response: " + responseText);
                    Debug.Log(res[0]);
                    Debug.Log(res[0].translations[0]);
                    Debug.Log(res[0].translations[0].text);
                    string[] resultStrings = res[0].translations[0].text.Split("<|>");
                    caption.SetTranslatedTitle(resultStrings[0]);
                    caption.SetTranslatedDescription(resultStrings[1]);
                    callback(caption);
                }
            }
        }
    }
}
