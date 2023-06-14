using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

public class CubeBehavior : MonoBehaviour, IMixedRealityGestureHandler
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("started");
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("Updated");
    }

    public void OnGestureStarted(InputEventData eventData)
    {
        Debug.Log("Gesture started");
    }

    public void OnGestureUpdated(InputEventData eventData)
    {
        Debug.Log("Gesture updated");
    }
 
    public void OnGestureCompleted(InputEventData eventData)
    {
        Debug.Log("Gesture completed");
    }

    public void OnGestureCanceled(InputEventData eventData)
    {
        Debug.Log("Gesture canceled");
    }
}

public class ChatGPTClient
{

}