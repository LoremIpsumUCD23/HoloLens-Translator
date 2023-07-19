using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using TTS;

public class DescriptionPanel : MonoBehaviour
{

    public TextMeshPro titleText;
    public TextMeshPro descriptionText;
    public GameObject switchButtonObject;
    public TextToSpeech ttsObject;

    public Caption captionRef;

    public bool translateDescription = false;

    public void SetText()
    {
        titleText.text = translateDescription ? captionRef.GetTranslatedTitle() : captionRef.GetPrimaryTitle();
        descriptionText.text = translateDescription ? captionRef.GetTranslatedDescription() : captionRef.GetPrimaryDescription();
    }

    public void ToggleDescription(bool flagCheck)
    {
        TextMeshPro buttonText = switchButtonObject.GetComponentInChildren<TextMeshPro>();
        buttonText.text = translateDescription ? "Switch to Translation" : "Switch to Primary";
        ttsObject.SetLocalAccent(translateDescription);
        translateDescription = !translateDescription;
        SetText();
    }

    public void PlayAudioDescription()
    {
        ttsObject.PlayAudio(titleText.text+"\n\n"+ descriptionText.text);
    }
}
