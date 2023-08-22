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

        //Dictionary for TTS language code
        public Dictionary<string, string> langDict = new Dictionary<string, string>{
            { "en", "en-GB" },
            { "ja","ja-JP" },
            { "bn","ba-IN" },
            { "hi","hi-IN" },
            { "ms","ms-MY" },
            { "es","es-ES" },
            { "fr","fr-FR" }
        };

        private AudioSource audioSource;

        private string otherLanguage = "";

        void Start()
        {
            audioSource = gameObject.GetComponent<AudioSource>();

            SetConfigAndLanguage();
        }

        public void StopAudio()
        {
            if (audioSource != null)
            {
                audioSource.Stop();
            }
        }
        /// <summary>
        /// Setter for secondary language audio
        /// </summary>
        /// <param name="otherLanguageString"></param>
        public void SetOtherLanguage(string otherLanguageString)
        {
            otherLanguage = langDict[otherLanguageString];
            Debug.Log("TTS: "+ otherLanguage); 
        }

        /// <summary>
        /// Getter for secondary language audio
        /// </summary>
        /// <returns></returns>
        public string GetOtherLanguage()
        {
            return otherLanguage;
        }

        /// <summary>
        /// Set language accent based on the language in view
        /// </summary>
        /// <param name="translateDescription"></param>
        public void SetLocalAccent(bool translateDescription)
        {
            localAccent = translateDescription ? "en-GB" : otherLanguage;

            config.SpeechSynthesisLanguage = localAccent;
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