using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Translator
{
    public interface ITranslatorClient
    {
        public IEnumerator Translate(string content, string language, Action<string> callback);
    }
}
