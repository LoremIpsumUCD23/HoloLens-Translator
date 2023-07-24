using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UI;

/// <summary>
/// Language Trigger that contains the language list for the application.
/// It sends the information to the LanguageManager on initialisation.
/// </summary>
public class InitialLanguageTrigger : MonoBehaviour
{
    // Set the available languages here
    private string[] _languages = { "English", "Malay", "Japanese", "Bengali", "Hindi" };
    // Set the translated text here
    private string[] _text = { "This sets the secondary language for the descriptions provided.", "Malay Text", "Japanese Text", "Bengali Text", "Hindi Text" };

    private List<Languages> _languageList;
    private Languages _language;

    // Initialise the language list
    //void Awake()
    //{
    //    _languageList = new List<Languages>();
    //    for (int i = 0; i < _languages.Length;  i++)
    //    {
    //        _language = new Languages();
    //        _language.language = _languages[i];
    //        _language.text = _text[i];
    //        _languageList.Add(_language);
    //        Debug.Log(_language.text);
    //    }
    //}

    /// <summary>
    /// Method to send this class's information to the LanguageManager
    /// </summary>
    public void TriggerLanguageInitialisation()
    {
        FindObjectOfType<LanguageManager>().StartLanguageInitialisation(_languageList);
    }
}
