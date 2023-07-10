using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UI;
public class LanguageTrigger : MonoBehaviour
{
    private List<Languages> _languageList;
    private string[] languages = { "English", "Malay", "Chinese" };
    private string[] text = { "English Text", "Malay Text", "Chinese Text" };
    private Languages language;

    void Awake()
    {
        language = new Languages();
        _languageList = new List<Languages>();
        for (int i = 0; i < languages.Length;  i++)
        {
            language.language = languages[i];
            language.text = text[i];
            _languageList.Add(language);
            Debug.Log(language.text);
        }
        Debug.Log(_languageList);
        Debug.Log(_languageList[0].language);
        Debug.Log(_languageList[1].language);
        Debug.Log(_languageList[2].language);
    }


    public void TriggerLanguageInitialisation()
    {
        FindObjectOfType<LanguageManager>().StartLanguageInitialisation(_languageList);
    }
}
