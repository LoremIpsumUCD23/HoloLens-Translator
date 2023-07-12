using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using TMPro;

public class SphereBehavior : MonoBehaviour, IMixedRealitySpeechHandler
{
    public TextMeshPro VoiceCommand;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Sphere Behavior Script started");
    }

    void IMixedRealitySpeechHandler.OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        Debug.Log("Voice Command: " + eventData.Command.Keyword);
        VoiceCommand.text = "Voice Command: " + eventData.Command.Keyword;
    }
}
