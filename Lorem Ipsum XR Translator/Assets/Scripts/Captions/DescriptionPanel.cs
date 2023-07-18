using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DescriptionPanel : MonoBehaviour
{

    public TextMeshPro titleText;
    public TextMeshPro descriptionText;

    public Caption captionRef;

    public bool translateDescription = false;

    public void SetText()
    {
        titleText.text = captionRef.GetTranslatedTitle() + " / " + captionRef.GetPrimaryTitle();
        descriptionText.text = translateDescription ? captionRef.GetTranslatedDescription() : captionRef.GetPrimaryDescription();
    }

    public void ToggleDescription()
    {
        translateDescription = !translateDescription;
        SetText();
    }
}
