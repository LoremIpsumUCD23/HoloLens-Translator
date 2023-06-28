using System;
using System.Collections;

namespace Description
{
    public interface IDescriptionClient
    {
        public IEnumerator SendRequest(string content, Action<string> callback);
    }
}
