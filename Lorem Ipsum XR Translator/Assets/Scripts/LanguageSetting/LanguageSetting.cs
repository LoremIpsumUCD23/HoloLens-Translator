using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

// Unity class to handle the language settings
public class LanguageSetting : MonoBehaviour
{
    // The different buttons for confirming, cancelling and selecting a language
    public PressableButton confirmButton;
    public PressableButton cancelButton;
    public PressableButton englishButton;
    public PressableButton hindiButton;
    public PressableButton malayButton;
    public PressableButton japaneseButton;
    public PressableButton bengaliButton;
    
    // A dictionary is used to store languages as keys and their corresponding Azure language codes as values
    private Dictionary<string, string> _languageCodeDictionary = new Dictionary<string, string>();

    // Variable to store the selected language
    private string selectedLanguage = ""; 

    // Method to return the selected language
    public string GetSelectedLanguage()
    {
        return selectedLanguage;
    }

    // You can then access the selected language from another script like below
    // string language = gameObject.GetComponent<LanguageSetting>().GetSelectedLanguage();

    /// <summary>
    /// Returns the Azure language code for the selected language.
    /// </summary>
    /// <returns></returns>
    public string GetSelectedLanguageCode()
    {
        return _languageCodeDictionary[GetSelectedLanguage()];
    }

    // Method called on the start of the game
    private void Start()
    {
        // Add listeners to the buttons for when they are pressed
        confirmButton.ButtonPressed.AddListener(ConfirmLanguageSelection);
        cancelButton.ButtonPressed.AddListener(CancelLanguageSelection);
        englishButton.ButtonPressed.AddListener(() => SetLanguage("English"));
        hindiButton.ButtonPressed.AddListener(() => SetLanguage("Hindi"));
        malayButton.ButtonPressed.AddListener(() => SetLanguage("Malay"));
        japaneseButton.ButtonPressed.AddListener(() => SetLanguage("Japanese"));
        bengaliButton.ButtonPressed.AddListener(() => SetLanguage("Bengali"));

        // Add the languages we want to support to the dictionary on start
        _languageCodeDictionary.Add("English", "en");
        _languageCodeDictionary.Add("Hindi", "hi");
        _languageCodeDictionary.Add("Malay", "ms");
        _languageCodeDictionary.Add("Japanese", "ja");
        _languageCodeDictionary.Add("Bengali", "bn");

    }

    // Method to set the selected language
    private void SetLanguage(string language)
    {
        selectedLanguage = language;
        Debug.Log("Language selected: " + language);
    }

    // Method to confirm the selected language
    private void ConfirmLanguageSelection()
    {
        // Check if a language has been selected before confirming
        if (string.IsNullOrEmpty(selectedLanguage))
        {
            Debug.Log("No language selected. Unable to confirm.");
            return;
        }

        Debug.Log("Language confirmed: " + selectedLanguage);
    }

    // Method to cancel the language selection
    private void CancelLanguageSelection()
    {
        Debug.Log("Language selection cancelled.");
    }
}
