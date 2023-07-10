using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using UI;

namespace UI
{
    public class LanguageManager : MonoBehaviour
    {
        public TextMeshPro Language;
        public TextMeshPro Text;

        private List<Languages> languagelist;
        private int indexer;
        private int size;

        public void DisplayNextLanguage()
        {
            Language.text = languagelist[indexer].language;
            Text.text = languagelist[indexer].text;
            if (indexer < size - 1) {
                indexer++;
            }
            else 
            {
                indexer = 0; 
            }
        }


        // Used to initialise new set of languages
        public void StartLanguageInitialisation(List<Languages> newlanguagelist)
        {
            Debug.Log("Starting Language Initialisation");
            indexer = 0;
            this.languagelist = newlanguagelist;
            size = this.languagelist.Count;
            Language.text = languagelist[indexer].language;
            Text.text = languagelist[indexer].text;
            indexer++;
        }
    }

    
}