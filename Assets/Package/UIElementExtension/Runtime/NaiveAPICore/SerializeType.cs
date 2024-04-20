using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NaiveAPI.UITK
{
    [Serializable]
    public class SerializeType : ISerializationCallbackReceiver
    {
        public Type value = null;
        [SerializeField] private string s_type;


        private static readonly Dictionary<string, Type> typeTable = new();
        public void OnAfterDeserialize()
        {
            if (!typeTable.TryGetValue(s_type, out value))
            {
                value = Type.GetType(s_type);
                if (value != null)
                    typeTable.Add(s_type, value);
            }
            s_type = null;
        }

        public void OnBeforeSerialize()
        {
            s_type = value.AssemblyQualifiedName;
        }
    }
}
