using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ObjectDetection
{
    public class AzureObjectDetector : IObjectDetectorClient
    {
        private readonly string subscriptionKey;
        private string azureObjApi;

        //initializing object with apiKey
        public AzureObjectDetector(string apiKey)
        {
            this.subscriptionKey = apiKey;
        }

        //calling the Azure API to return response
        public IEnumerator DetectObjects(string modelPath, string imagePath, Action<string> callback)
        {
            byte[] byteData = System.Text.Encoding.UTF8.GetBytes("{\"url\":\"" + imagePath + "\"}");

                //UnityWebRequest client = new UnityWebRequest(modelPath, "POST");

                using (UnityWebRequest client = new UnityWebRequest(modelPath, "POST"))
                {
                    client.uploadHandler = new UploadHandlerRaw(byteData);
                    client.downloadHandler = new DownloadHandlerBuffer();

                    // Request headers
                    client.SetRequestHeader("Content-Type", "application/json");
                    client.SetRequestHeader("Ocp-Apim-Subscription-Key", subscriptionKey);

                    yield return client.SendWebRequest();

                    if (client.result != UnityWebRequest.Result.Success)
                    {
                        Debug.Log("API request failed. Error: " + client.error);
                        callback("object");
                    }
                    else
                    {
                        string responseText = client.downloadHandler.text;
                        Debug.Log("API response: " + responseText);
                        callback(responseText);
                    }
            }
        }
    }
}