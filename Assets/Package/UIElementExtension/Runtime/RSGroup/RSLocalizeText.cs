using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace NaiveAPI.UITK
{
    [System.Serializable]
    public class RSLocalizeText : ISerializationCallbackReceiver
    {
        public static RSLocalizeText FromKey(string key)
        {
            return new RSLocalizeText { key = key };
        }
        public static RSLocalizeText FromValue(string value)
        {
            return new RSLocalizeText { value = value };
        }
        public FontAsset FontAsset
        {
            get
            {
                _Reload();
                return m_FontAsset;
            }
        }
        private FontAsset m_FontAsset;
        public string value
        {
            get
            {
                _Reload();
                return m_value;
            }
            set
            {
                isBind = false;
                m_value = value;
            }
        }
        [SerializeField] private string m_value;
        public bool IsBinding { get => isBind; set => isBind = value; }
        [SerializeField]private bool isBind = false;
        private bool isDirty = true;
        [SerializeField] private string m_key;
        public string key
        {
            get => m_key;
            set
            {
                m_key = value;
                isDirty = true;
                isBind = true;
            }
        }
        private void _Reload()
        {
            if (!isBind)
                return;
            if (!isDirty)
                return;
            isDirty = false;
            (m_value, m_FontAsset) = RSLocalization.GetTextAndFont(m_key);
        }
        public void Reload()
        {
            if (isBind)
                (m_value, m_FontAsset) = RSLocalization.GetTextAndFont(m_key);
        }
        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize() { isDirty = true; }
    }
}
