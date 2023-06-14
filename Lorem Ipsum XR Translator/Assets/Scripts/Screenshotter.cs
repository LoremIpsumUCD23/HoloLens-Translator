using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Screenshotter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void TakeScreenshot()
    {
        Debug.Log("Try Screenshot!");
        Texture2D screenCap = ScreenCapture.CaptureScreenshotAsTexture(ScreenCapture.StereoScreenCaptureMode.BothEyes);

        byte[] imageData = screenCap.EncodeToPNG();
        string dirPath = Application.dataPath + "/../Screenshots/";
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllBytes(dirPath + "Screenshot.png", imageData);
    }

}
