using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageSetting : MonoBehaviour
{
    // Create public fields to assign your buttons in the Inspector
    public Button confirmButton;
    public Button cancelButton;
    public Button englishButton;
    public Button hindiButton;
    public Button malayButton;
    public Button japaneseButton;
    public Button bengaliButton;
    
    // This will store the language selected by the user
    private string selectedLanguage = "";

    private void Start()
    {
        // Add listeners to your buttons
        confirmButton.onClick.AddListener(ConfirmLanguageSelection);
        cancelButton.onClick.AddListener(CancelLanguageSelection);
        englishButton.onClick.AddListener(() => SetLanguage("English"));
        hindiButton.onClick.AddListener(() => SetLanguage("Hindi"));
        malayButton.onClick.AddListener(() => SetLanguage("Malay"));
        japaneseButton.onClick.AddListener(() => SetLanguage("Japanese"));
        bengaliButton.onClick.AddListener(() => SetLanguage("Bengali"));
    }

    // This function sets the selected language
    private void SetLanguage(string language)
    {
        selectedLanguage = language;
        Debug.Log("Language selected: " + language);
    }

    // This function confirms the language selection and applies it
    private void ConfirmLanguageSelection()
    {
        if (string.IsNullOrEmpty(selectedLanguage))
        {
            Debug.Log("No language selected. Unable to confirm.");
            return;
        }

        // Call your localization system's function to apply the language here
        LocalizationSystem.SetLanguage(selectedLanguage);
        Debug.Log("Language confirmed: " + selectedLanguage);
    }

    // This function cancels the language selection
    private void CancelLanguageSelection()
    {
        selectedLanguage = "";
        Debug.Log("Language selection cancelled.");
    }
}
