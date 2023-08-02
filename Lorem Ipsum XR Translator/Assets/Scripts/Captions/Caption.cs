using TMPro;
using UnityEngine;

public class Caption : MonoBehaviour
{
    public TextMeshPro textObject;

    public CaptionLibrary CaptionLib;
    public CaptionController CaptionCon;

    string secondaryLanguage;

    public string primaryTitle;
    public string translatedTitle;
    public string primaryDescription;
    public string translatedDescription;
    public bool feedbackSent;

    public void SetPrimaryTitle(string pTitle) { primaryTitle = pTitle; SetText(); }

    public string GetPrimaryTitle() { return primaryTitle; }

    public void SetTranslatedTitle(string tTitle) { translatedTitle = tTitle; SetText(); }

    public string GetTranslatedTitle() { return translatedTitle; }

    public void SetPrimaryDescription(string pDesc) { primaryDescription = pDesc; SetText(); }

    public string GetPrimaryDescription() { return primaryDescription; }

    public void SetTranslatedDescription(string tDesc) { translatedDescription = tDesc; SetText(); }

    public string GetTranslatedDescription() { return translatedDescription; }

    public void InitializeCaption(string pTitle, CaptionLibrary captionLib, CaptionController captionCon)
    {
        primaryTitle = pTitle;
        SetPrimaryDescription("");
        SetTranslatedTitle("");
        SetTranslatedDescription("");
        CaptionLib = captionLib;
        CaptionCon = captionCon;
        feedbackSent = false;
        SetText();
    }

    /// <summary>
    /// If caption does not have its texts set yet, request them from the caption library.
    /// </summary>
    void Update()
    {
        // Make sure we have our associated caption library
        if (CaptionLib)
        {
            string holdString = CaptionLib.HoldString;
            // If description is not set, try to get it from captionlibrary.
            if (string.IsNullOrEmpty(primaryDescription) || primaryDescription == holdString)
            {
                primaryDescription = CaptionLib.TryGetDescription(primaryTitle);
                SetText();
            }
            // Get and set translated title
            if (string.IsNullOrEmpty(translatedTitle) || translatedTitle == holdString)
            {
                translatedTitle = CaptionLib.TryGetTitleTranslation(primaryTitle);
                SetText();
            }
            // Get and set translated description
            if (string.IsNullOrEmpty(translatedDescription) || translatedDescription == holdString)
            {
                translatedDescription = CaptionLib.TryGetDescriptionTranslation(primaryTitle);
                SetText();
            }
        }
    }

    /// <summary>
    /// Sets the location of the caption
    /// </summary>
    /// <param name="location">The location to put the caption at</param>
    public void SetLocation(Vector3 location)
    {
        transform.position = location;
    }

    /// <summary>
    /// Sets the text on the associated UI object.
    /// </summary>
    private void SetText()
    {
        if (textObject == null)
        {
            textObject = transform.GetComponentInChildren<TextMeshPro>();
        }
        if (!string.IsNullOrEmpty(translatedTitle))
        {
            textObject.text = translatedTitle;
            //Debug.Log(textObject.text);
        }
        else // Set our caption with the primary text while we await our translation.
        {
            textObject.text = primaryTitle;
            //Debug.Log(textObject.text);
        }
    }

    /// <summary>
    /// Toggles the caption object active/inactive
    /// </summary>
    /// <param name="active">True = active, false = inactive</param>
    public void Activate(bool active)
    {
        gameObject.SetActive(active);
    }

    /// <summary>
    /// Called when the caption object is interacted with.
    /// Sends the caption details to the description panel
    /// </summary>
    public void Interact()
    {
        if (CaptionCon.DescriptionPanel != null)
        {
            DescriptionPanel descriptionPanel = CaptionCon.DescriptionPanel.GetComponent<DescriptionPanel>();
            if (descriptionPanel.ttsObject != null)
            {
                descriptionPanel.StopAudioCall();
            }
            descriptionPanel.captionRef = this;
            descriptionPanel.SetSecondaryLanguage(CaptionLib.GetSecondaryLanguage());
            descriptionPanel.SetText();
            CaptionCon.DescriptionPanel.SetActive(true);
            if (!feedbackSent)
            {
                descriptionPanel.feedbackSection.SetActive(true);
            }
        }
    }
}
