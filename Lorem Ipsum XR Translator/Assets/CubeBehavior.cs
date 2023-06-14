using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

// use it for sending requests to ChatGPT
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;

public class CubeBehavior : MonoBehaviour, IMixedRealityGestureHandler
{

    private ChatGPTClient _client;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Started");
        this._client = new ChatGPTClient("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", "gpt-3.5-turbo");
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("Updated");
    }

    public void OnGestureStarted(InputEventData eventData)
    {
        Debug.Log("Gesture Started");
        Debug.Log("Call ChatGPT API");

        string res = this._client.SendPrompt("Hello, ChatGPT!").Result;
        Debug.Log(res);
    }

    public void OnGestureUpdated(InputEventData eventData)
    {
        Debug.Log("Gesture Updated");
    }
 
    public void OnGestureCompleted(InputEventData eventData)
    {
        Debug.Log("Gesture Completed");
        Debug.Log("Call ChatGPT API");
        string res = this._client.SendPrompt("Hello, ChatGPT!").Result;
        Debug.Log(res);
    }

    public void OnGestureCanceled(InputEventData eventData)
    {
        Debug.Log("Gesture Canceled");
    }
}


public class ChatGPTClient
{ 
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;

    public ChatGPTClient(string apiKey, string model)
    {
        this._apiKey = apiKey;
        this._httpClient = new HttpClient();
        this._httpClient.BaseAddress = new Uri("https://api.openai.com/v1/");
        this._httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        this._model = model;
    }

    public string Test()
    {
        return "This is test";
    }

    public async Task<string> SendPrompt(string prompt)
    {
        var req = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/completions");

        // Headers
        // req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", this._apiKey);

        // JSON body data
        var reqBody = new { prompt = prompt, model = this._model, max_tokens = 150, temperature = 0.5 };

        // Convert the body data to JSON and add to the request
        req.Content = new StringContent(JsonConvert.SerializeObject(reqBody), Encoding.UTF8, "application/json");

        HttpResponseMessage res = await this._httpClient.SendAsync(req);

        if (res.IsSuccessStatusCode)
        {
            return await res.Content.ReadAsStringAsync();
        }
        else
        {
            return "Error: " + res.StatusCode;
        }
    }

}