using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;
using System.Net.Http;


namespace Translator
{

public class AzureTranslator : ITranslatorClient
{
    //private static readonly string key = "5878a06b7c2c4a66beed0915fe52a400";
    private readonly string endpoint = "https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&";

    // location, also known as region.
    // required if you're using a multi-service or regional (not global) resource. It can be found in the Azure portal on the Keys and Endpoint page.
    //private static readonly string location = "northeurope";
    private readonly string apiKey;
    private readonly string  location;

    public AzureTranslator(string apikey ,string location)
    {
        // private const string Url = "https://api.openai.com/v1/completions";

        this.apiKey = apiKey;
        this.location = location;
        
    }


    public IEnumerator Translate(string originalText, string from, string[] to, Action<string> callback)
    {
        string url = endpoint + string.Format("from={0}", from);
        for (int i = 0; i < to.Length; i ++)
        {
            url += string.Format("&to={0}", to[i]);
        }
        Debug.Log(url);

        // Create the request body
        string requestBody = "[{ \"Text\": \"" + text + "\" }]";
        byte[] requestData = System.Text.Encoding.UTF8.GetBytes(requestBody);

        www.uploadHandler = new UploadHandlerRaw(requestData);
    
        // Create a UnityWebRequest object
        UnityWebRequest request = UnityWebRequest.Post(url, requestBody);
    
        // Set the request headers
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Ocp-Apim-Subscription-Key", apiKey);
        www.SetRequestHeader("Ocp-Apim-Subscription-Region",location); 

        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
    
        // Send the request
        yield return request.SendWebRequest();
    
        // Handle the response
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error: " + request.error);
            callback("Error: " + request.error);
        }
        else
        {
            string responseText = request.downloadHandler.text;
            // Parse and process the response as needed
            Debug.Log("Translation response: " + responseText);
            callback(responseText);
        }
    }

    //public static async Task Main()
    //{
        // Input and output languages are defined as parameters.
    //    string route = "/translate?api-version=3.0&from=en&to=lan1&to=ja";
    //    string textToTranslate = "Hi , my name is tintin";
         
    //    object[] body = new object[] { new { Text = textToTranslate } };
    //    var requestBody = JsonConvert.SerializeObject(body);

    //    using (var client = new HttpClient())
    //    using (var request = new HttpRequestMessage())
    //    {
            // Build the request.
    //        request.Method = HttpMethod.Post;
    //        request.RequestUri = new Uri(endpoint + route);
    //        request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
    //        request.Headers.Add("Ocp-Apim-Subscription-Key", key);
            // location required if you're using a multi-service or regional (not global) resource.
    //        request.Headers.Add("Ocp-Apim-Subscription-Region", location);

            // Send the request and get response.
    //        HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
            // Read response as a string.
    //        string result = await response.Content.ReadAsStringAsync();
           // Console.WriteLine(result);
    //        Debug.Log(result);
    //    }
    }
}
