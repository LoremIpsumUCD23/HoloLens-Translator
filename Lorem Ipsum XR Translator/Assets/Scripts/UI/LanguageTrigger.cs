using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UI;
public class LanguageTrigger : MonoBehaviour
{
    private List<Languages> _languageList;
    private string[] languages = { "English", "Malay", "Japanese", "Bengali", "Hindi" };
    private string[] text = { "This sets the secondary language for the descriptions provided.", "Malay Text", "Japanese Text", "Bengali Text", "Hindi Text" };
    private Languages language;

    void Awake()
    {
        _languageList = new List<Languages>();
        for (int i = 0; i < languages.Length;  i++)
        {
            language = new Languages();
            language.language = languages[i];
            language.text = text[i];
            _languageList.Add(language);
            Debug.Log(language.text);
        }
    }


    public void TriggerLanguageInitialisation()
    {
        FindObjectOfType<LanguageManager>().StartLanguageInitialisation(_languageList);
    }
}
