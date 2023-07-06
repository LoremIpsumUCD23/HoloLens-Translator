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
        titleText.text = captionRef.translatedTitle + " / " + captionRef.primaryTitle;
        descriptionText.text = translateDescription ? captionRef.translatedDescription : captionRef.primaryDescription;
    }

    public void ToggleDescription()
    {
        translateDescription = !translateDescription;
        SetText();
    }
}
