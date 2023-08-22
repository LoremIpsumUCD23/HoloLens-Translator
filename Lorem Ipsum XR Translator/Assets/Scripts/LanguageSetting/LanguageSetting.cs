using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;

// Unity class to handle the language settings
public class LanguageSetting : MonoBehaviour
{
    // The different buttons for confirming, cancelling and selecting a language
    public PressableButton firstConfirmButton;
    public PressableButton confirmButton;
    public PressableButton cancelButton;
    //public PressableButton englishButton;
    public PressableButton hindiButton;
    public PressableButton malayButton;
    public PressableButton japaneseButton;
    public PressableButton bengaliButton;
    public PressableButton spanishButton;
    public PressableButton frenchButton;

    // Field for Feedback Text
    [SerializeField] TextMeshPro FeedbackText;

    // A dictionary is used to store languages as keys and their corresponding Azure language codes as values
    private Dictionary<string, string> _languageCodeDictionary = new Dictionary<string, string>();

    // Variable to store the selected language
    private string selectedLanguage = "English";

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
        firstConfirmButton.ButtonPressed.AddListener(ConfirmLanguageSelection);
        confirmButton.ButtonPressed.AddListener(ConfirmLanguageSelection);
        cancelButton.ButtonPressed.AddListener(CancelLanguageSelection);
        //englishButton.ButtonPressed.AddListener(() => SetLanguage("English"));
        //hindiButton.ButtonPressed.AddListener(() => SetLanguage("Hindi"));
        //malayButton.ButtonPressed.AddListener(() => SetLanguage("Malay"));
        //japaneseButton.ButtonPressed.AddListener(() => SetLanguage("Japanese"));
        //bengaliButton.ButtonPressed.AddListener(() => SetLanguage("Bengali"));

        //englishButton.GetComponent<Interactable>().OnClick.AddListener(() => SetLanguage("English"));
        hindiButton.GetComponent<Interactable>().OnClick.AddListener(() => SetLanguage("Hindi"));
        malayButton.GetComponent<Interactable>().OnClick.AddListener(() => SetLanguage("Malay"));
        japaneseButton.GetComponent<Interactable>().OnClick.AddListener(() => SetLanguage("Japanese"));
        bengaliButton.GetComponent<Interactable>().OnClick.AddListener(() => SetLanguage("Bengali"));
        spanishButton.GetComponent<Interactable>().OnClick.AddListener(() => SetLanguage("Spanish"));
        frenchButton.GetComponent<Interactable>().OnClick.AddListener(() => SetLanguage("French"));
        


        // Add the languages we want to support to the dictionary on start
        _languageCodeDictionary.Add("English", "en");
        _languageCodeDictionary.Add("Hindi", "hi");
        _languageCodeDictionary.Add("Malay", "ms");
        _languageCodeDictionary.Add("Japanese", "ja");
        _languageCodeDictionary.Add("Bengali", "bn");
        _languageCodeDictionary.Add("Spanish", "es");
        _languageCodeDictionary.Add("French", "fr");

    }

    // Method to set the selected language
    private void SetLanguage(string language)
    {
        selectedLanguage = language;
        FeedbackText.text = "Current Language Selected : " + GetSelectedLanguage();
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
