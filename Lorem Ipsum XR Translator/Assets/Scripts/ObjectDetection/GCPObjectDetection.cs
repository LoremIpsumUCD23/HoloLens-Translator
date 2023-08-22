using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace ObjectDetection
{
    public class GCPObjectDetection : IObjectDetectorClient
    {
        private string url = "https://vision.googleapis.com/v1/images:annotate";
        private string apiKey;

        public GCPObjectDetection(string apiKey)
        {
            this.url = this.url + "?key=" + apiKey;
        }


        public IEnumerator DetectObjects(string modelPath, string imagePath, Action<List<DetectedObject>> callback)
        {
            throw new NotImplementedException();
        }



        public IEnumerator DetectObjects(string modelPath, Texture2D image, Action<List<DetectedObject>> callback)
        {

            byte[] imgBytes = image.EncodeToJPG();
            string base64Encoded = Convert.ToBase64String(imgBytes);

            string jsonString = $@"{{
            ""parent"": """",
            ""requests"": [
                {{
                    ""image"": {{
                        ""content"": ""{base64Encoded}""
                    }},
                    ""features"": [
                        {{
                            ""type"": ""LABEL_DETECTION""
                        }},
                        {{
                            ""type"": ""OBJECT_LOCALIZATION""
                        }},
                        {{
                            ""type"": ""SAFE_SEARCH_DETECTION""
                        }},
                        {{
                            ""type"": ""PRODUCT_SEARCH""
                        }}
                    ]
                }}
            ]}}";

            byte[] byteData = System.Text.Encoding.UTF8.GetBytes(jsonString);

            // Create a web request and send it (along with our bytedata) to the Azure recognition service
            using (UnityWebRequest client = new UnityWebRequest(this.url, "POST"))
            {
                client.uploadHandler = new UploadHandlerRaw(byteData);
                client.downloadHandler = new DownloadHandlerBuffer();

                // Request headers
                client.SetRequestHeader("Content-Type", "application/octet-stream");
                client.SetRequestHeader("Accept", "*/*");

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
                    string res = client.downloadHandler.text;
                    GCPObjDetectionResponse resData = new GCPObjDetectionResponse(res, image.width, image.height);


                    foreach (DetectedObject obj in resData.objects)
                    {
                        Debug.Log("Response: " + obj.objectName);
                    }

                    // Send detected object list to callback
                    callback(resData.objects);
                }
            }
        }
    }
}
