using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NaiveAPI.UITK
{
    [System.Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField][HideInInspector] private TKey[] s_keys;
        [SerializeField][HideInInspector] private TValue[] s_values;
        public void OnAfterDeserialize()
        {
            if(s_keys != null && s_values != null)
            {
                for (int i = 0; i < s_keys.Length; i++)
                    Add(s_keys[i], s_values[i]);
            }
            s_keys = null;
            s_values = null;
        }

        public void OnBeforeSerialize()
        {
            s_keys = new TKey[Count];
            s_values = new TValue[Count];
            int i= 0;
            foreach(var pair in this)
            {
                s_keys[i] = pair.Key;
                s_values[i] = pair.Value;
                i++;
            }
        }
    }
}
