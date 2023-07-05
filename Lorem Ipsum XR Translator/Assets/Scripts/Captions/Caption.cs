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

    public void InitializeCaption(string pTitle, string tTitle, string pDesc, string tDesc)
    {
        primaryTitle = pTitle;
        translatedTitle = tTitle;
        primaryDescription = pDesc; 
        translatedDescription = tDesc;

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
}
