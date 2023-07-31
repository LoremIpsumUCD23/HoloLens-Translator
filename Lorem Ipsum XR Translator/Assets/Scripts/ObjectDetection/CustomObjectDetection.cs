using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace ObjectDetection
{
    public class CustomObjectDetection : IObjectDetectorClient
    {
        private string host;

        public CustomObjectDetection(string host)
        {
            this.host = host;
        }

        public IEnumerator DetectObjects(string modelPath, string imagePath, Action<List<DetectedObject>> callback)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Detects objects recognized in a Texture2D image
        /// </summary>
        /// <param name="modelPath">model name</param>
        /// <param name="image">Texture2D image to be analyzed (will be cast to a JPG)</param>
        /// <param name="callback">Callback function to send list of detected objects</param>
        /// <returns>IEnumerator waits for web response</returns>
        public IEnumerator DetectObjects(string modelPath, Texture2D image, Action<List<DetectedObject>> callback)
        {
            // Encode our Texture2D to a byte array of JPG data
            byte[] byteData = image.EncodeToJPG();

            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            formData.Add(new MultipartFormFileSection("image", byteData, "image.jpg", "image/jpeg"));

            // Create a web request and send it (along with our bytedata) to the Azure recognition service
            using (UnityWebRequest client = UnityWebRequest.Post(this.host + "/detect?model=" + modelPath, formData))
            {
                client.downloadHandler = new DownloadHandlerBuffer();

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

                    // Parse detected objects from JSON to objects
                    CustomObjDetectionResponse responseData = new CustomObjDetectionResponse(responseTextApi);
                    List<DetectedObject> detectedObjects = responseData.objects;

                    Debug.Log(responseTextApi);

                    // Send detected object list to callback
                    callback(detectedObjects);
                }
            }
        }
    }
}

