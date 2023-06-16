using System;
using System.Collections;

namespace Translator
{
    public interface ITranslatorClient
    {
        public IEnumerator Translate(string originalText, string language, Action<string> callback);
    }
}
