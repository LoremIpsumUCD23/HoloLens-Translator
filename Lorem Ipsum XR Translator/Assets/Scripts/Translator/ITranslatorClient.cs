using System;
using System.Collections;

namespace Translator
{
    public interface ITranslatorClient
    {
        // from: original language
        // to: list of target languages
        public IEnumerator Translate(string originalText, string from, string[] to, Action<string> callback);
    }
}
