using System;
using System.Collections;

namespace Description
{
    public interface IDescriptionClient
    {
        public IEnumerator SendRequest(Caption caption, Action<Caption> callback);
    }
}
