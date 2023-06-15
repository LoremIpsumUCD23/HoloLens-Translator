using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using TMPro;

// our own namespace
using Description;

public class CubeBehavior : MonoBehaviour, IMixedRealityGestureHandler
{
    public TextMeshPro textObject;

    private IDescriptionAPIClient _client;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Started");
        textObject.text = "Sugai";

        // TODO: Check your API KEY here: https://platform.openai.com/account/api-keys
        this._client = new ChatGPTClient("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", "text-davinci-003");
        StartCoroutine(this._client.SendRequest("Good morning!", this.getDescription));
    }

    public void OnGestureStarted(InputEventData eventData)
    {
        Debug.Log("Gesture Started");
        StartCoroutine(this._client.SendRequest("Good afternoon!", this.getDescription));
    }

    public void OnGestureUpdated(InputEventData eventData)
    {            
        Debug.Log("Gesture Updated");
    }
 
    public void OnGestureCompleted(InputEventData eventData)
    {
        Debug.Log("Gesture Completed");
        StartCoroutine(this._client.SendRequest("Good evening!", this.getDescription));
    }

    public void OnGestureCanceled(InputEventData eventData)
    {
        Debug.Log("Gesture Canceled");
    }

    private void getDescription(string responseText)
    {
        if (responseText != null)
        {
            textObject.text = responseText;
        }
        else
        {
            Debug.Log("Got null. Must be something wrong with Description API client's implementation");
        }
    }
}
