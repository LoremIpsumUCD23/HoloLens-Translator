using System;
using System.Collections.Generic;

public static class LocalizationSystem
{
    public static Dictionary<string, string> localizedText;
    // For this example, we'll use English as the default language
    private static string defaultLanguage = "English";
    private static string currentLanguage;

    public static void SetLanguage(string language)
    {
        currentLanguage = language;

        // Load translations from a file or a database, or any other source
        // This is just a simplified example
        localizedText = new Dictionary<string, string>();

        if (language == "English") 
        {
            localizedText.Add("greeting", "Hello");
            // Add all the other translations
        }
        else if (language == "Hindi")
        {
            localizedText.Add("greeting", "नमस्ते");
            // Add all the other translations
        }
        else if (language == "Japanese")
        {
            localizedText.Add("greeting", "こんにちは");
            // Add all the other translations
        }
        else if (language == "Malay")
        {
            localizedText.Add("greeting", "Halo");
            // Add all the other translations
        }
        else if (language == "Bengali")
        {
            localizedText.Add("greeting", "হ্যালো");
            // Add all the other translations
        }
    }

    public static string GetCurrentLanguage()
    {
        if (string.IsNullOrEmpty(currentLanguage))
            return defaultLanguage;
        else
            return currentLanguage;
    }

    public static string GetLocalizedText(string key)
    {
        string localizedValue = "";
        if (localizedText.TryGetValue(key, out localizedValue))
            return localizedValue;
        else
            return key; // return the key itself if no translation was found
    }

    public static void ResetLanguage()
    {
        currentLanguage = "";
        localizedText.Clear();
    }
}
