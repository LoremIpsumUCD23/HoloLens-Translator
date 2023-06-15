using System;
using System.Collections;

namespace Description
{
    public interface IDescriptionAPIClient
    {
        public IEnumerator SendRequest(string content, Action<string> callback);
    }
}
