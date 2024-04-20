using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace NaiveAPI.UITK
{
    [CreateAssetMenu(menuName = "Naive API/Localization Table")]
    public class SO_RSLocalizationTable : ScriptableObject//, ISerializationCallbackReceiver
    {
        public string LanguageKey = "";
        public FontAsset FontAsset;
        public SerializableDictionary<string, string> TextTable = new();
        public SerializableDictionary<string, Sprite> SpriteTable = new();

        //private string[] s_text_key;
        //private string[] s_text_value;
        //private string[] s_sprite_key;
        //private Sprite[] s_sprite_value;
        //public void OnBeforeSerialize()
        //{
        //    s_text_key = new string[TextTable.Count];
        //    s_text_value = new string[TextTable.Count];
        //    s_sprite_key = new string[SpriteTable.Count];
        //    s_sprite_value = new Sprite[SpriteTable.Count];
        //    int i = 0;
        //    foreach (var item in TextTable)
        //    {
        //        s_text_key[i] = item.Key;
        //        s_text_value[i++] = item.Value;
        //    }
        //    i = 0;
        //    foreach (var item in SpriteTable)
        //    {
        //        s_sprite_key[i] = item.Key;
        //        s_sprite_value[i++] = item.Value;
        //    }
        //}

        //public void OnAfterDeserialize()
        //{
        //    if(s_text_key != null && s_text_value != null)
        //    {
        //        for (int i = 0; i < s_text_key.Length; i++)
        //            TextTable.Add(s_text_key[i], s_text_value[i]);
        //    }
        //    if (s_sprite_key != null && s_sprite_value != null)
        //    {
        //        for (int i = 0; i < s_sprite_key.Length; i++)
        //            SpriteTable.Add(s_sprite_key[i], s_sprite_value[i]);
        //    }
        //    s_text_key = null;
        //    s_text_value = null;
        //    s_sprite_key = null;
        //    s_sprite_value = null;
        //}
    }
}
