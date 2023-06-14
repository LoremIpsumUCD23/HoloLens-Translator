using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Screenshotter : MonoBehaviour
{
    /// <summary>
    /// Captures a screenshot of the user's current view.
    /// </summary>
    public void TakeScreenshot()
    {
        Debug.Log("Try Screenshot!");
        Texture2D screenCap = ScreenCapture.CaptureScreenshotAsTexture(ScreenCapture.StereoScreenCaptureMode.BothEyes);


        // Right now we save our screenshot to a file.
        // Later we will want to send it to Azure Vision for analysis instead.
        SaveScreenshotToFile(screenCap);
    }

    /// <summary>
    /// Writes a Texture2D to a PNG file and saves it to storage.
    /// </summary>
    /// <param name="screenshotData"></param>
    void SaveScreenshotToFile(Texture2D screenCap)
    {
        // Convert Texture2D to binary byte array with PNG file format.
        byte[] imageData = screenCap.EncodeToPNG();

        // Check if Screenshots directory exists. If not, create it.
        string dirPath = Application.dataPath + "/../Screenshots/";
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        // Write our screenshot data to the screenshot file.
        File.WriteAllBytes(dirPath + "Screenshot.png", imageData);

    }

}
