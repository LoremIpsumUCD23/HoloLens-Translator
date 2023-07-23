using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


/// <summary>
/// Manages description of objects in a static class
/// </summary>

public static class DescriptionManager
{

    private static string _secondaryLanguage; // chosen secondary language
    private static string _title = "cube";
    private static string _translationText;
    private static string _dictionaryText;
    private static string _chatGPTText;

    // public getters and setters
    public static string SecondaryLanguage { get; set; }
    public static string Title { get; set; }
    public static string TranslationText { get; set; }
    public static string DictionaryText { get; set; }
    public static string ChatGPTText { get; set; }

    /// <summary>
    /// Public method to set a new description
    /// </summary>
    /// <param name="title">Word of the object</param>
    /// <param name="translation">Translated description</param>
    /// <param name="dictionary">Dictionary description</param>
    /// <param name="chatgpt">ChatGPT description</param>
    public static void SetNewDescription(string title, string translation, string dictionary, string chatgpt)
    {
        DescriptionManager.Title = title;
        DescriptionManager.TranslationText = translation;
        DescriptionManager.DictionaryText = dictionary;
        DescriptionManager.ChatGPTText = chatgpt;
    }
}

