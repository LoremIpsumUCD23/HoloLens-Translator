using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UI
{
    /// <summary>
    /// Description behaviour that can be applied to description panels.
    /// Retrieves the description from the description manager.
    /// </summary>
    public class DescriptionBehavior : MonoBehaviour
    {
        // set the text to be altered in unity
        [SerializeField] private TextMeshPro _title;
        [SerializeField] private TextMeshPro _translationText;
        [SerializeField] private TextMeshPro _dictionaryText;
        [SerializeField] private TextMeshPro _chatGPTText;
        [SerializeField] private GameObject DescriptionPanel;

        /// <summary>
        /// Retrieves description from the Description Manager
        /// </summary>
        public void RetrieveDescription()
        {
            Debug.Log("Retrieving Description");
            _title.text = DescriptionManager.Title;
            _translationText.text = DescriptionManager.TranslationText;
            _dictionaryText.text = DescriptionManager.DictionaryText;
            _chatGPTText.text = DescriptionManager.ChatGPTText;
            Debug.Log(_dictionaryText.text);
            DescriptionPanel.SetActive(true);
        }
    }
}
