using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DescriptionPanel : MonoBehaviour
{

    public TextMeshPro titleText;
    public TextMeshPro descriptionText;
    public GameObject switchButtonObject;

    public Caption captionRef;

    public bool translateDescription = false;

    public void SetText()
    {
        titleText.text = captionRef.GetTranslatedTitle() + " / " + captionRef.GetPrimaryTitle();
        descriptionText.text = translateDescription ? captionRef.GetTranslatedDescription() : captionRef.GetPrimaryDescription();
    }

    public void ToggleDescription(bool flagCheck)
    {
        TextMeshPro buttonText = switchButtonObject.GetComponentInChildren<TextMeshPro>();
        buttonText.text = translateDescription ? "Switch to Translation" : "Switch to Primary";
        Debug.Log(translateDescription);
        translateDescription = !translateDescription;
        SetText();
    }
}
