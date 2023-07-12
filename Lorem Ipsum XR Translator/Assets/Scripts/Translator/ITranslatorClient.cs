using System;
using System.Collections;

namespace Translator
{
    /// <summary>
    /// Interface ITranslatorClient provides methods related to translation.
    /// </summary>
    public interface ITranslatorClient
    {
        /// <summary>
        /// Translates <paramref name="originalText"/> from <paramref name="from"/> to <paramref name="to"/>, and do <paramref name="callback"> based on the translation result.
        /// </summary>
        /// <param name="originalText">Text that gets translated.</param>
        /// <param name="from">The language which <paramref name="originalText"/> is written in.</param>
        /// <param name="to">Target language(s) which <paramref name="originalText"/> is translated into.</param>
        /// <param name="callback">An action that gets executed with the translated text.</param>
        public IEnumerator Translate(string originalText, string from, string[] to, Action<string[]> callback);
    }
}
