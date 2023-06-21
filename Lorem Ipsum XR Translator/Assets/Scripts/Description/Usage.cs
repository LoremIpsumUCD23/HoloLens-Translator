using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Description
{
    [Serializable]
    class Usage
    {
        public int prompt_tokens;
        public int completion_tokens;
        public int total_tokens;
    }
}