using System;

namespace Config
{
    public class Secrets
    {
        public static string GetChatGPTApiKey()
        {
            return "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";
        }

        public static string GetDictApiKeyFor(string level)
        {
            switch (level)
            {
                case "elementary":
                    return "50975cc1-b4e7-4666-af66-03067dc6060f";
                case "intermediate":
                    return "6009aa88-c0ad-49ef-97fe-c2e785c7d0a8";
                default:
                    throw new Exception(String.Format("Level {0} is not supported", level));
            }
        }

        public static string GetAzureTranslatorKey()
        { 
            return "5878a06b7c2c4a66beed0915fe52a400";
        }
    }
}

