using Microsoft.CognitiveServices.Speech;
using System;
using UnityEngine;
 
public class TextToSpeech : MonoBehaviour
{
    SpeechConfig config;
    string speechSynthesisLanguage = "ja-JP" ; 
     string speakText = "Hi August"  ;  
     string subscriptionKey = "1272512de9d14e4ebcab8f56b9bfaeb1" ;
     string region = "northeurope" ;
 
    void Start()
    {
        //Set Text To Speech
        config = SpeechConfig.FromSubscription(subscriptionKey, region);
        config.SpeechSynthesisLanguage = speechSynthesisLanguage; // Set the language to speak 
        config.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Raw16Khz16BitMonoPcm);
        //SynthesizeAudioAsync();
    }

 
    public async void SynthesizeAudioAsync()
    {
        try
        {
            // convert text to speech and get result 
            using var synthesizer = new SpeechSynthesizer(config, null );
            var result = await synthesizer.SpeakTextAsync(speakText);
    
            //Play the acquired audio with the Audio Source component
            var audioSource = gameObject.AddComponent<AudioSource>();
            var sampleCount = result.AudioData.Length / 2;
            var audioData = new float[sampleCount];
            for (var i = 0; i < sampleCount; ++i)
            {
                audioData[i] = (short)(result.AudioData[i * 2 + 1] << 8 | result.AudioData[i * 2]) / 32768.0F;
                Debug.Log("Audio Data[" + i + "] : " + audioData[i]); // Debug line
            }
            var audioClip = AudioClip.Create("SynthesizedAudio", sampleCount, 1, 16000, false);
            audioClip.SetData(audioData, 0);
            audioSource.clip = audioClip;
            audioSource.Play();
    
            Debug.Log("Success");
        }
        catch(Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }


}