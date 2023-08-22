using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;


namespace Feedback
{
    public class FeedbackClient
    {
        private string host;

        public FeedbackClient(string host)
        {
            this.host = host;
        }

        /// <summary>
        /// Sends a POST request to a specific service endpoint, providing feedback about a model.
        /// </summary>
        /// <param name="service">The type of service. Must be one of the following: "detection", "translation", "description".</param>
        /// <param name="model">The name of the model for which the feedback is being given.</param>
        /// <param name="isPositive">A boolean indicating whether the feedback is positive (true) or not (false).</param>
        /// <returns>An IEnumerator instance, required for coroutine execution.</returns>
        /// <example>
        /// An example call might look like: StartCoroutine(PutFeedback("detection", "chatgpt", true));
        /// </example>
        public IEnumerator PutFeedback(string service, string model, bool isPositive)
        {
            var data = new { model = model, is_positive = isPositive };

            string jsonString = JsonConvert.SerializeObject(data);

            Debug.Log("Feedback Request: " + jsonString);

            byte[] byteData = System.Text.Encoding.UTF8.GetBytes(jsonString);

            // Create a web request and send it (along with our bytedata) to the Azure recognition service
            using (UnityWebRequest client = new UnityWebRequest(host + "/feedback/" + service, "POST"))
            {
                client.uploadHandler = new UploadHandlerRaw(byteData);
                client.downloadHandler = new DownloadHandlerBuffer();

                // Request headers
                client.SetRequestHeader("Content-Type", "application/json");

                // Wait for web response
                yield return client.SendWebRequest();

                // Process response
                if (client.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log("API request failed. Error: " + client.error);
                }
                else
                {
                    Debug.Log("Feedback success!");
                }
            }
        }
    }
}

