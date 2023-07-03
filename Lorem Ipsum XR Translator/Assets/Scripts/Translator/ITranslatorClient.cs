using System;
using System.Collections;

namespace Translator
{
    public interface ITranslatorClient
    {
        // from: original language
        // to: list of target languages
        public IEnumerator Translate(Caption caption, string from, string[] to, Action<Caption> callback);
    }
}
