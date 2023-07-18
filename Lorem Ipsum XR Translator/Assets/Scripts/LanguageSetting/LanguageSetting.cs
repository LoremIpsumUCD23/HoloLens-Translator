using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.UI;

public class LanguageSetting : MonoBehaviour
{
    public PressableButton confirmButton;
    public PressableButton cancelButton;
    public PressableButton englishButton;
    public PressableButton hindiButton;
    public PressableButton malayButton;
    public PressableButton japaneseButton;
    public PressableButton bengaliButton;
    
    private string selectedLanguage = "";

    private void Start()
    {
        confirmButton.ButtonPressed.AddListener(ConfirmLanguageSelection);
        cancelButton.ButtonPressed.AddListener(CancelLanguageSelection);
        englishButton.ButtonPressed.AddListener(() => SetLanguage("English"));
        hindiButton.ButtonPressed.AddListener(() => SetLanguage("Hindi"));
        malayButton.ButtonPressed.AddListener(() => SetLanguage("Malay"));
        japaneseButton.ButtonPressed.AddListener(() => SetLanguage("Japanese"));
        bengaliButton.ButtonPressed.AddListener(() => SetLanguage("Bengali"));
    }

    private void SetLanguage(string language)
    {
        selectedLanguage = language;
        Debug.Log("Language selected: " + language);
    }

    private void ConfirmLanguageSelection()
    {
        if (string.IsNullOrEmpty(selectedLanguage))
        {
            Debug.Log("No language selected. Unable to confirm.");
            return;
        }

        LocalizationSystem.SetLanguage(selectedLanguage);
        Debug.Log("Language confirmed: " + selectedLanguage);
    }

    private void CancelLanguageSelection()
    {
        selectedLanguage = "";
        Debug.Log("Language selection cancelled.");
    }
}
