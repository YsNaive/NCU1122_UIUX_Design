using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using static NaiveAPI.UITK.SO_RSLocalizationTable;

namespace NaiveAPI.UITK
{
    public static class RSLocalization
    {
        public class SR : RSLocalizationKeyProvider
        {
            public const string name = "naiveapi.name";
            public const string icon = "naiveapi.icon";
            public const string label = "naiveapi.label";
            public const string colorPicker = "naiveapi.colorPicker";
            public const string cancel = "naiveapi.cancel";

            public override IEnumerable<string> TextKeys
            {
                get
                {
                    yield return name;
                    yield return icon;
                    yield return label;
                    yield return colorPicker;
                    yield return cancel;
                }
            }

            public override IEnumerable<string> ImageKeys => Enumerable.Empty<string>();
        }
        public static IEnumerable<string> LanguageKeys => UIElementExtensionResource.Get.LanguageKeys;
        public static HashSet<string> TextKeys => UIElementExtensionResource.Get.LanguageTextKeys;
        public static HashSet<string> SpriteKeys => UIElementExtensionResource.Get.LanguageImageKeys;
        public static List<SO_RSLocalizationTable> LoadedTable => UIElementExtensionResource.Get.PreloadLangeaueTable;

        public static IEnumerable<string> FindNotDefinedTextKey(string languageKey)
        {
            var matchedTable = LoadedTable.Where(m => m.LanguageKey == languageKey).ToArray();
            return TextKeys.Where(key => !matchedTable.Any(table => table.TextTable.ContainsKey(key)));
        }
        public static IEnumerable<string> FindNotDefinedSpriteKey(string languageKey)
        {
            var matchedTable = LoadedTable.Where(m => m.LanguageKey == languageKey).ToArray();
            return SpriteKeys.Where(key => !matchedTable.Any(table => table.SpriteTable.ContainsKey(key)));
        }

        public static string DefaultLanguage
        {
            get => UIElementExtensionResource.Get.DefaultLanguage;
            set => UIElementExtensionResource.Get.DefaultLanguage = value;
        }
        public static string CurrentLanguage
        {
            get => UIElementExtensionResource.Get.CurrentLanguage;
            set => UIElementExtensionResource.Get.CurrentLanguage = value;
        }

        public static readonly Dictionary<string, Dictionary<string, (string, FontAsset)>> RuntimeTextTable = new();
        public static readonly Dictionary<string, Dictionary<string, Sprite>> RuntimeSpriteTable = new();
        public static Dictionary<string, (string, FontAsset)> GetTextTable(string languageKey)
        {
            languageKey ??= "";
            if (RuntimeTextTable.TryGetValue(languageKey, out var table))
                return table;
            table = new();
            RuntimeTextTable.Add(languageKey, table);
            return table;
        }
        public static Dictionary<string, Sprite> GetSpriteTable(string languageKey)
        {
            languageKey ??= "";
            if (RuntimeSpriteTable.TryGetValue(languageKey, out var table))
                return table;
            table = new ();
            RuntimeSpriteTable.Add(languageKey, table);
            return table;
        }
        public static string GetText(string key) { return GetTextAndFont(key).text; }
        public static (string text, FontAsset font) GetTextAndFont(string key)
        {
            if (GetTextTable(CurrentLanguage).TryGetValue(key, out var ret))
                return ret;
            if (CurrentLanguage != DefaultLanguage)
            {
                if (GetTextTable(DefaultLanguage).TryGetValue(key, out ret))
                    return ret;
            }
            return (key, null);
        }
        public static Sprite GetSprite(string key)
        {
            if (GetSpriteTable(CurrentLanguage).TryGetValue(key, out var ret))
                return ret;
            if (CurrentLanguage != DefaultLanguage)
            {
                if (GetSpriteTable(DefaultLanguage).TryGetValue(key, out ret))
                    return ret;
            }
            return ret;
        }
        static RSLocalization()
        {
            foreach(var table in LoadedTable)
            {
                if (table == null)
                    continue;
                        
                var textDst = GetTextTable(table.LanguageKey);
                var spriteDst = GetSpriteTable(table.LanguageKey);
                foreach(var textPair in table.TextTable)
                {
                    if (textPair.Value == null) return;
                    if (textDst.ContainsKey(textPair.Key))
                        continue;
                    textDst.Add(textPair.Key, (textPair.Value, table.FontAsset));
                }
                foreach (var spritePair in table.SpriteTable)
                {
                    if (spritePair.Value == null) return;
                    if (spriteDst.ContainsKey(spritePair.Key))
                        continue;
                    spriteDst.Add(spritePair.Key, spritePair.Value);
                }
            }
        }

        [System.Serializable]
        public class LanguageInfo
        {
            public string key = "";
            public string name = "";
            public Sprite icon = null;
        }
    }
}
