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

        public AzureObjectDetector(string apiKey)
        {
            this.subscriptionKey = apiKey;
        }

        public IEnumerator DetectObjects(string modelPath, string imagePath, Action<string> callback)
        {
            byte[] byteData = System.Text.Encoding.UTF8.GetBytes("{\"url\":\"" + imagePath + "\"}");

            UnityWebRequest client = new UnityWebRequest(modelPath, "POST");

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
                string responseTextApi = client.downloadHandler.text;
                string responseText = "";
                
                AzureObjDetectionResponse responseData = new AzureObjDetectionResponse(responseTextApi);
                List<DetectedObject> detectedObjects = responseData.objects;

                foreach (DetectedObject detectedObject in detectedObjects)
                {
                    Debug.Log("Response: " + detectedObject.objectName);
                    responseText = responseText + detectedObject.objectName + ": " + detectedObject.confidence + "\n";
                }
                Debug.Log("API response: " + responseText);
                callback(responseText);
            }
        }
    }
}