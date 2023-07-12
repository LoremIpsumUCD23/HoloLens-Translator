using Microsoft.CognitiveServices.Speech;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

using Config;

namespace TTS
{
    [RequireComponent(typeof(AudioSource))]
    public class TextToSpeech : MonoBehaviour , ITextToSpeech
    {
        // Availble: ja-JP, en-GB, ba-IN, hi-IN, ms-MY;
        private string localAccent = "en-GB";

        private SpeechConfig config;

        void Start()
        {
            // Set key and region
            this.config = SpeechConfig.FromSubscription(Secrets.GetTTSSpeechSdkKey(), "northeurope");

            // Set accent
            this.config.SpeechSynthesisLanguage = localAccent; // Set the language to speak

            // 
            config.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Raw16Khz16BitMonoPcm);
        }

        public async void PlayAudio(string speakText)
        {
            try
            {
                // convert text to speech and get result 
                using (var synthesizer = new SpeechSynthesizer(this.config, null))
                {
                    var result = await synthesizer.SpeakTextAsync(speakText);
                    //Play the acquired audio with the Audio Source component
                    var audioSource = gameObject.GetComponent<AudioSource>();
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
            }
            catch (Exception e)
            {
                // TODO: Error handling properly.
                Debug.Log(e.Message);
            }
        }
    }
}