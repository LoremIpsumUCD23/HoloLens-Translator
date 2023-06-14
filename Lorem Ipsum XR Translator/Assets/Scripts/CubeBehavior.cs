using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

// use it for sending requests to ChatGPT
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;

public class CubeBehavior : MonoBehaviour, IMixedRealityGestureHandler
{
    public TextMeshPro text;

    private ChatGPTClient _client;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Started");
        text.text = "Sugai";

        // TODO: Check your API KEY here: https://platform.openai.com/account/api-keys
        this._client = new ChatGPTClient("PUT YOUR API KEY HERE", "text-davinci-003");
        this.updateText("Good morning");
    }

    // Update is called once per frame
    //void Update()
    //{
    //   Debug.Log("Updated");
    //}

    public void OnGestureStarted(InputEventData eventData)
    {
        Debug.Log("Gesture Started");
        text.text = "Gesture Started";
        this.updateText("Say hey");
    }

    public void OnGestureUpdated(InputEventData eventData)
    {
        Debug.Log("Gesture Updated");
        text.text = "Gesture Updated";
        this.updateText("Say hi");
    }
 
    public void OnGestureCompleted(InputEventData eventData)
    {
        Debug.Log("Gesture Completed");
        text.text = "Gesture Completed";
        this.updateText("Say yo");
    }

    public void OnGestureCanceled(InputEventData eventData)
    {
        Debug.Log("Gesture Canceled");
    }

    public async void updateText(string greeting)
    {
        Debug.Log("Call ChatGPT API");
        text.text = await this._client.SendPrompt(greeting);
    }
}

          
public class ChatGPTClient
{ 
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;

    public ChatGPTClient(string apiKey, string model)
    {
        this._httpClient = new HttpClient();
        this._httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);
        this._model = model;
    }

    public async Task<string> SendPrompt(string prompt)
    {
        // JSON body data
        var reqBody = new PromptRequest(this._model, prompt, 10, 0.0f);
        string reqBodyJson = JsonUtility.ToJson(reqBody);

        // Convert the body data to JSON and add to the request
        HttpContent content = new StringContent(reqBodyJson, Encoding.UTF8, "application/json");
        
        HttpResponseMessage res = await this._httpClient.PostAsync("https://api.openai.com/v1/completions", content);
        Debug.Log("ChatGPTClient: Got a response");

        if (res.IsSuccessStatusCode)
        {
            Debug.Log("ChatGPTClient: Reading the response");
            string resContent = await res.Content.ReadAsStringAsync();
            Debug.Log("ChatGPTClient: Response Text: " + resContent);
            return resContent;
        }
        else
        {
            Debug.Log("ChatGPTClient: Error Status Code received: " + res.StatusCode + ". Message: " + res.ReasonPhrase);
            return "error";
        }
    }
}

[Serializable]
public class PromptRequest
{
    public string model;
    public string prompt;
    public int max_tokens;
    public float temperature;

    public PromptRequest(string model, string prompt, int max_tokens, float temperature)
    {
        this.model = model;
        this.prompt = prompt;
        this.max_tokens = max_tokens;
        this.temperature = temperature;
    }
}

[Serializable]
public class Choice
{
    public string text;
    public int index;
    public string finish_reason;
}

[Serializable]
public class Usage
{
    public int prompt_tokens;
    public int completion_tokens;
    public int total_tokens;
}

[Serializable]
public class Response
{
    public string id;
    public string object_type;
    public int created;
    public string model;
    public List<Choice> choices;
    public Usage usage;
}