using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using TTS;

using Config;


public class DescriptionPanel : MonoBehaviour
{

    public TextMeshPro titleText;
    public TextMeshPro descriptionText;
    public GameObject switchButtonObject;
    public TextToSpeech ttsObject;
    public GameObject detectionFeedbackSection;
    public GameObject descriptionFeedbackSection;

    string secondaryLanguage;

    public Caption captionRef;

    public bool translateDescription = false;

    /// <summary>
    /// Setter for all the text on the description panel based on selected captions
    /// </summary>
    public void SetText()
    {
        ttsObject.SetOtherLanguage(secondaryLanguage);
        titleText.text = translateDescription ? captionRef.GetTranslatedTitle() : captionRef.GetPrimaryTitle();
        descriptionText.text = translateDescription ? captionRef.GetTranslatedDescription() : captionRef.GetPrimaryDescription();
    }

    /// <summary>
    /// Setter for secondary language
    /// </summary>
    /// <param name="langString"></param>
    public void SetSecondaryLanguage(string langString)
    {
        secondaryLanguage = langString;
    }

    /// <summary>
    /// Toggle description panel text based on selection
    /// </summary>
    /// <param name="flagCheck"></param>
    public void ToggleDescription(bool flagCheck)
    {
        TextMeshPro buttonText = switchButtonObject.GetComponentInChildren<TextMeshPro>();
        buttonText.text = translateDescription ? "Switch to Translation" : "Switch to English";
        ttsObject.SetLocalAccent(translateDescription);
        translateDescription = !translateDescription;
        SetText();
    }

    /// <summary>
    /// Play description panel audio
    /// </summary>
    public void PlayAudioDescription()
    {
        ttsObject.PlayAudio(titleText.text+"\n\n"+ descriptionText.text);

    }

    public void StopAudioCall()
    {
        ttsObject.StopAudio();
    }

    public void RemoveDetectionFeedbackSection()
    {
        if (detectionFeedbackSection != null)
        {
            detectionFeedbackSection.SetActive(false);
        }
        captionRef.detectionFeedbackSent = true;
    }


    public void RemoveDescriptionFeedbackSection()
    {
        if (descriptionFeedbackSection != null)
        {
            descriptionFeedbackSection.SetActive(false);
        }
        captionRef.descriptionFeedbackSent = true;
    }

}
