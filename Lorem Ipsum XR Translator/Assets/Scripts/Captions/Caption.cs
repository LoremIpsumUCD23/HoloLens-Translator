using TMPro;
using UnityEngine;

public class Caption : MonoBehaviour
{
    // Just public for editor testing
    public string primaryTitle;
    public string translatedTitle;
    public string primaryDescription;
    public string translatedDescription;
    public TextMeshPro textObject;

    public CaptionLibrary CaptionLib;
    public CaptionController CaptionCon;

    public void InitializeCaption(string pTitle, CaptionLibrary captionLib, CaptionController captionCon)
    {
        primaryTitle = pTitle;
        CaptionLib = captionLib;
        CaptionCon = captionCon;
        SetText();
    }

    void Update()
    {
        if (CaptionLib)
        {
            string holdString = CaptionLib.HoldString;
            if (string.IsNullOrEmpty(primaryDescription) || primaryDescription == holdString)
            {
                primaryDescription = CaptionLib.TryGetDescription(primaryTitle);
                SetText();
            }
            if (string.IsNullOrEmpty(translatedTitle) || translatedTitle == holdString)
            {
                translatedTitle = CaptionLib.TryGetTitleTranslation(primaryTitle);
                SetText();
            }
            if (string.IsNullOrEmpty(translatedDescription) || translatedDescription == holdString)
            {
                translatedDescription = CaptionLib.TryGetDescriptionTranslation(primaryTitle);
                SetText();
            }
        }
    }

    public void SetPrimaryTitle(string pTitle) { primaryTitle = pTitle; SetText(); }

    public string GetPrimaryTitle() { return primaryTitle; }

    public void SetTranslatedTitle(string tTitle) { translatedTitle = tTitle; SetText(); }

    public string GetTranslatedTitle() {  return translatedTitle; }

    public void SetPrimaryDescription(string pDesc) { primaryDescription = pDesc; SetText(); }

    public string GetPrimaryDescription() { return primaryDescription; }

    public void SetTranslatedDescription(string tDesc) {  translatedDescription = tDesc; SetText(); }

    public string GetTranslatedDescription() { return translatedDescription; }

    public void SetLocation(Vector3 location)
    {
        transform.position = location;
    }

    private void SetText()
    {
        if (textObject == null)
        {
            textObject = transform.Find("CaptionText").GetComponent<TextMeshPro>();
        }
        if (!string.IsNullOrEmpty(translatedTitle))
        {
            textObject.text = translatedTitle;
        }
        else // Set our caption with the primary text while we await our translation.
        {
            textObject.text = primaryTitle;
        }
    }

    public void Activate(bool active)
    {
        gameObject.SetActive(active);
    }

    public void Interact()
    {
        DescriptionPanel descriptionPanel = CaptionCon.DescriptionPanel.GetComponent<DescriptionPanel>();
        descriptionPanel.captionRef = this;
        descriptionPanel.SetText();
        CaptionCon.DescriptionPanel.SetActive(true);
    }
}
