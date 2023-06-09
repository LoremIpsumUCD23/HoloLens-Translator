using Microsoft.CognitiveServices.Speech;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
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

        private AudioSource audioSource;

        void Start()
        {
            audioSource = gameObject.GetComponent<AudioSource>();

            SetConfigAndLanguage();
        }

        void SetConfigAndLanguage()
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
    SpeechSynthesisResult audioData = await GetAudioData(speakText);
    Debug.Log(audioData);
    if (audioData == null)
    {
        Debug.Log("An Error occurred");
        return;
    }
    int sampleCount = audioData.AudioData.Length / 2;
    AudioClip audioClip = AudioClip.Create("SynthesizedAudio", sampleCount, 1, 16000, false);
    float[] resultData = new float[sampleCount];
    for (int i = 0; i < sampleCount; ++i)
    {
        resultData[i] = (short)(audioData.AudioData[i * 2 + 1] << 8 | audioData.AudioData[i * 2]) / 32768.0f; 
        //Debug.Log("Audio Data[" + i + "] : " + resultData[i]); // Debug line
    }
    audioClip.SetData(resultData, 0);
    
    // To check if the AudioSource is enabled, if not, will enable it
    if (!audioSource.enabled)
    {
        audioSource.enabled = true;
    }

    audioSource.clip = audioClip;
    audioSource.Play();
}

        async Task<SpeechSynthesisResult> GetAudioData(string speakText)
        {
            try
            {
                if (config == null)
                {
                    SetConfigAndLanguage();
                }
                // convert text to speech and get result 
                using (var synthesizer = new SpeechSynthesizer(this.config, null))
                {
                    SpeechSynthesisResult result = await synthesizer.SpeakTextAsync(speakText);
                    Debug.Log("Success");
                    return result;
                }
            }
            catch (Exception e)
            {
                // TODO: Error handling properly.
                Debug.Log(e.Message);
                return null;
            }

        }
    }
}