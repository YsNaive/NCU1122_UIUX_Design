using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NaiveAPI.UITK
{
    [System.Serializable]
    public class SerializableHashSet<T> : HashSet<T>, ISerializationCallbackReceiver
    {
        [SerializeField][HideInInspector]
        private T[] s_data;
        public SerializableHashSet():base() { }
        public void OnAfterDeserialize()
        {
            if (s_data != null)
            {
                foreach (T item in s_data)
                   Add(item);
            }
            s_data = null;
        }
        public void OnBeforeSerialize()
        {
            s_data = new T[Count];
            int i = 0;
            foreach(var item in this)
                s_data[i++] = item;
        }
    }
}
