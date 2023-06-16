using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace Description
{
    public class DictionaryAPIClient : IDescriptionClient
    {
        public DictionaryAPIClient()
        {
            // TODO: complete this constructor
        }

        public IEnumerator SendRequest(string content, Action<string> callback)
        {
            // TODO: complete this method
            // callback("Put a resulting string or error message here at the end of this method.");
            return null;
        }
    }
}
