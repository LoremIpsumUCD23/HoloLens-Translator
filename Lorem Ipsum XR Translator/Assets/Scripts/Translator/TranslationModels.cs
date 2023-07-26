using System;

[System.Serializable]
public class TranslationResponse
{
    public TranslationItem[] translations;
}

[System.Serializable]
public class TranslationItem
{
    public string text;
    public string to;
}
