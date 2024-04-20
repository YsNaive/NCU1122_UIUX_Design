using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NaiveAPI.UITK
{
    public abstract class RSLocalizationKeyProvider
    {
        public abstract IEnumerable<string> TextKeys { get; }
        public abstract IEnumerable<string> ImageKeys { get; }
    }
}
