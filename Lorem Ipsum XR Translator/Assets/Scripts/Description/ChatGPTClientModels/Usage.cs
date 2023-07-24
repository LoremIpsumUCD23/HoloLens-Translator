using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Description
{
    /// <summary>
    /// Usage info contained in a response from OpenAI api.
    /// </summary>
    [Serializable]
    class Usage
    {
        public int prompt_tokens;
        public int completion_tokens;
        public int total_tokens;
    }
}