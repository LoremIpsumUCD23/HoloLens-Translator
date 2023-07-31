using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;


namespace Translator
{

    /// </summary>
    /// Class AzureTranslator is used to get translation on various languages through Azure Text Translator api
    /// </summary>
    public class AzureTranslator : ITranslatorClient
    {
        private const string endpoint = "https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&";


        /// </summary>
        /// location, also known as region for the api service.
        /// required if you're using a multi-service or regional (not global) resource. It can be found in the Azure portal on the Keys and Endpoint page.
        /// <see href="https://api.cognitive.microsofttranslator.com/translate?api-version=3.0" /> has been used to aceess the api
        /// </summary>
        private readonly string apiKey;
        private readonly string location;

        public AzureTranslator(string apiKey,string location)
        {
            this.apiKey = apiKey;
            this.location = location;
        }


        public IEnumerator Translate(string originalText, string from, string[] to, Action<string> callback)
        {
            string url = AzureTranslator.endpoint + string.Format("from={0}", from);
            for (int i = 0; i < to.Length; i ++)
            {
                url += string.Format("&to={0}", to[i]);
            }

            /// </summary>
            /// Here,The request body as how it will be displayed has been created
            /// </summary>
            string requestBody = "[{ \"Text\": \"" + originalText + "\" }]";

            /// </summary>
            /// Here,a new UnityWebRequest object is created, using the constructor that takes a URL and HTTP verb (POST) as parameters. 
            ///The URL is specified as url.
            /// </summary>
            using (UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
            {
                byte[] requestData = System.Text.Encoding.UTF8.GetBytes(requestBody);
                request.uploadHandler = new UploadHandlerRaw(requestData);

                /// </summary>
                /// Request is used to set the request headers.SetRequestHeader.
                /// The Content-Type header is set to "application/json" in this case, suggesting that the request body is in JSON format. 
                ///It also populates the Ocp-Apim-Subscription-Key and Ocp-Apim-Subscription-Region headers with values from this.apiKey and this.location, respectively.
                /// </summary>
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Ocp-Apim-Subscription-Key", this.apiKey);
                request.SetRequestHeader("Ocp-Apim-Subscription-Region", this.location);

                /// </summary>
                ///request.downloadHandler is set to a new DownloadHandlerBuffer object. 
                ///This instructs the response to be downloaded into a buffer for later processing.
                /// </summary>
                request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();

                // Send the request
                yield return request.SendWebRequest();

                // Request sent, clean up our request data to prevent a memory leak.
                // TODO: See if there's a better way to handle this. This seems like it could be expensive.
                requestData = null;
                GC.Collect();

                /// </summary>
                /// The reponse is  indicating an error has occurred, a debug log message is printed with the error message, 
                /// and the provided callback function is called with the error message.
                /// </summary>
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log("Error: " + request.error);
                    callback("Error: " + request.error);
                }
                else
                {
                    /// </summary>
                    /// In request.downloadHandler.text. This implies that the response will be in text format. 
                    ///The response text is then logged using a debug log message, and the supplied callback function is invoked.
                    /// </summary>
                    string responseText = request.downloadHandler.text;
                    Debug.Log("Translation response: " + responseText);
                    callback(responseText);
                }
            }
        }
    }
}
