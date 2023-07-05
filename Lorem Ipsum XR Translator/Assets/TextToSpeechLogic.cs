using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using HoloToolkit.Unity;
// using HoloToolkit.Unity.InputModule;


public class TextToSpeechLogic : MonoBehaviour, IInputClickHandler
{
   private TextToSpeech textTospeech;
   public string speakText;

   private void Awake()
   {
    textTospeech = GetComponent<TextToSpeech>();
   } 

   public void OnInputClicked(InputClickedEventData eventData)
   {
    var msg= string.Format(
    SpeakerText, textToSpeech.Voice.ToString());

    textToSpeech.StartSpeaking(msg);

   }
}
