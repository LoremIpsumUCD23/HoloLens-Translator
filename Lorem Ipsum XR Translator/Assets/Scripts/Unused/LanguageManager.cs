using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using UI;

namespace UI
{
    /// <summary>
    /// This class manages the language and corresponding text to be displayed on the language selection panel.
    /// </summary>
    public class LanguageManager : MonoBehaviour
    {
        // set the text to be altered in unity
        public TextMeshPro Language;
        public TextMeshPro Text;

        // contains the language list to iterate over
        private List<Languages> languagelist;

        // for indexing purposes
        private int indexer;
        private int size;

        /// <summary>
        /// Displays the next set of language
        /// </summary>
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

        /// <summary>
        /// Used to initialise new set of languages
        /// </summary>
        /// <param name="newlanguagelist"> A language list to be set. Initially it is set by the InitialLanguageTrigger script</param>
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