using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TTS;

public class TestTTS : MonoBehaviour
{
    public TextToSpeech TTS;

    string captionText = "Test text";

    // Start is called before the first frame update
    void Start()
    {
        if (TTS == null)
        {
            TTS = GameObject.Find("TextToSpeech").GetComponent<TextToSpeech>();
        }

        PlayCaption();
    }

    public void PlayCaption()
    {
        TTS.PlayAudio(captionText);
    }

}
