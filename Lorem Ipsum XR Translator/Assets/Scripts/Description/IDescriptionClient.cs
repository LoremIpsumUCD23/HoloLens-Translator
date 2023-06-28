using System;
using System.Collections;

namespace Description
{
    public interface IDescriptionClient
    {
        public IEnumerator Explain(string content, Action<string> callback);
    }
}
