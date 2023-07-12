using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ObjectDetection
{
    /// <summary>
    /// This class calls Azure Object Detection Service and receives the responses
    /// </summary>
    public class AzureObjectDetector : IObjectDetectorClient
    {
        private readonly string subscriptionKey;
        private string azureObjApi;
        private string modelPath;

        public AzureObjectDetector(string apiKey, string modelPath)
        {
            this.subscriptionKey = apiKey;
            this.modelPath = modelPath;
        }

        /// <summary>
        /// Detects objects in an image from a provided URL
        /// </summary>
        /// <param name="imagePath">URL for image to be analyzed</param>
        /// <param name="callback">Callback function to send list of detected objects</param>
        /// <returns>IEnumerator waits for web response</returns>
        public IEnumerator DetectObjects(string imagePath, Action<List<DetectedObject>> callback)
        {
            // Collect byte data from image at URL
            byte[] byteData = System.Text.Encoding.UTF8.GetBytes("{\"url\":\"" + imagePath + "\"}");

            // Create a web request and send it (along with our bytedata) to the Azure recognition service
            using (UnityWebRequest client = new UnityWebRequest(this.modelPath, "POST"))
            {
                client.uploadHandler = new UploadHandlerRaw(byteData);
                client.downloadHandler = new DownloadHandlerBuffer();

                // Request headers
                client.SetRequestHeader("Content-Type", "application/json");
                client.SetRequestHeader("Ocp-Apim-Subscription-Key", subscriptionKey);

                // Wait for web response
                yield return client.SendWebRequest();

                // If response fails, log error and call callback with empty list.
                if (client.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log("API request failed. Error: " + client.error);
                    callback(new List<DetectedObject>());
                }
                else // Successful analysis!
                {
                    string responseTextApi = client.downloadHandler.text;
                    string responseText = "";

                    // Parse detected objects from JSON to objects
                    AzureObjDetectionResponse responseData = new AzureObjDetectionResponse(responseTextApi);
                    List<DetectedObject> detectedObjects = responseData.objects;

                    // Log our detected objects
                    foreach (DetectedObject detectedObject in detectedObjects)
                    {
                        Debug.Log("Response: " + detectedObject.objectName);
                        responseText = responseText + detectedObject.objectName + ": " + detectedObject.confidence + "\n";
                    }
                    Debug.Log("API response: " + responseText);

                    // Send detected object list to callback
                    callback(detectedObjects);
                }
            }
        }
        /// <summary>
        /// Detects objects recognized in a Texture2D image
        /// </summary>
        /// <param name="image">Texture2D image to be analyzed (will be cast to a JPG)</param>
        /// <param name="callback">Callback function to send list of detected objects</param>
        /// <returns>IEnumerator waits for web response</returns>
        public IEnumerator DetectObjects(Texture2D image, Action<List<DetectedObject>> callback)
        {
            // Encode our Texture2D to a byte array of JPG data
            byte[] byteData = image.EncodeToJPG();

            // Create a web request and send it (along with our bytedata) to the Azure recognition service
            using (UnityWebRequest client = new UnityWebRequest(this.modelPath, "POST"))
            {
                client.uploadHandler = new UploadHandlerRaw(byteData);
                client.downloadHandler = new DownloadHandlerBuffer();

                // Request headers
                client.SetRequestHeader("Content-Type", "application/octet-stream");
                client.SetRequestHeader("Ocp-Apim-Subscription-Key", subscriptionKey);

                // Wait for web response
                yield return client.SendWebRequest();

                // If response fails, log error and call callback with empty list.
                if (client.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log("API request failed. Error: " + client.error);
                    callback(new List<DetectedObject>());
                }
                else // Successful analysis!
                {
                    string responseTextApi = client.downloadHandler.text;
                    string responseText = "";

                    // Parse detected objects from JSON to objects
                    AzureObjDetectionResponse responseData = new AzureObjDetectionResponse(responseTextApi);
                    List<DetectedObject> detectedObjects = responseData.objects;

                    // Log our detected objects
                    foreach (DetectedObject detectedObject in detectedObjects)
                    {
                        Debug.Log("Response: " + detectedObject.objectName);
                        responseText = responseText + detectedObject.objectName + ": " + detectedObject.confidence + "\n";
                    }
                    Debug.Log("API response: " + responseText);

                    // Send detected object list to callback
                    callback(detectedObjects);
                }
            }
        }
    }
}