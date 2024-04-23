using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NaiveAPI.UITK
{
    public class UIElementExtensionResource : ScriptableObject, ISerializationCallbackReceiver
    {
        public const string SourceFileName = "UIElementExtensionResource";
        public const string ResourceFolderGUID = "1d7f381a39e37624483edc51eb1cd3cb";
        public static UIElementExtensionResource Get {
            get
            {
                instance ??= Resources.Load<UIElementExtensionResource>(SourceFileName);
                instance ??= CreateInstance<UIElementExtensionResource>();
                return instance;
            }
        }
        private static UIElementExtensionResource instance;

        public SO_RSTheme DefaultTheme;
        public SO_RSTheme DarkTheme;
        public SO_RSTheme LightTheme;

        public IEnumerable<string> IgnoreAssemblyName => m_IgnoreAssemblyName;
        public Dictionary<Type, DefaultDrawerSettings> DefaultDrawerSettings = new();
        public static DefaultDrawerSettings Get_or_CreateDefaultDrawerSettings(Type type)
        {
            if(Get.DefaultDrawerSettings.TryGetValue(type, out var settings))
                return settings;
            settings = new DefaultDrawerSettings();
            Get.DefaultDrawerSettings.Add(type, settings);
            return settings;
        }

        public List<RSLocalization.LanguageInfo> LanguageInfos = new();
        public HashSet<string> LanguageTextKeys = new();
        public HashSet<string> LanguageImageKeys = new();
        public string DefaultLanguage = "en";
        public string CurrentLanguage = "en";
        public IEnumerable<string> LanguageKeys => LanguageInfos.Select(info => info.key);
        public List<SO_RSLocalizationTable> PreloadLangeaueTable = new();

        [SerializeField] List<string> m_IgnoreAssemblyName = new();
        [SerializeField] private List<string> s_DefaultDrawerSettings_keys = new();
        [SerializeField] private List<DefaultDrawerSettings> s_DefaultDrawerSettings_values = new();
        [SerializeField] private string[] s_LanguageTextKeys = new string[0];
        [SerializeField] private string[] s_LanguageImageKeys = new string[0];

        public void OnBeforeSerialize()
        {
            s_DefaultDrawerSettings_keys = new();
            s_DefaultDrawerSettings_values = new();
            s_DefaultDrawerSettings_keys.Capacity = DefaultDrawerSettings.Count;
            s_DefaultDrawerSettings_values.Capacity = DefaultDrawerSettings.Count;
            foreach (var pair in DefaultDrawerSettings)
            {
                if (pair.Value.IsDefault())
                    continue;
                s_DefaultDrawerSettings_keys.Add(pair.Key.AssemblyQualifiedName);
                s_DefaultDrawerSettings_values.Add(pair.Value);
            }

            s_LanguageTextKeys = new string[LanguageTextKeys.Count];
            int i = 0;
            foreach (var item in LanguageTextKeys)
                s_LanguageTextKeys[i++] = item;
            s_LanguageImageKeys = new string[LanguageImageKeys.Count];
            i = 0;
            foreach (var item in LanguageImageKeys)
                s_LanguageImageKeys[i++] = item;
            var set = new HashSet<SO_RSLocalizationTable>(PreloadLangeaueTable);
            PreloadLangeaueTable = set.Where(m => m != null).ToList();
        }
        public void OnAfterDeserialize()
        {
            for(int i = 0; i < s_DefaultDrawerSettings_keys.Count; i++)
            {
                DefaultDrawerSettings.TryAdd(Type.GetType(s_DefaultDrawerSettings_keys[i]), s_DefaultDrawerSettings_values[i]);
            }

            foreach (var item in s_LanguageTextKeys)
                LanguageTextKeys.Add(item);
            s_LanguageTextKeys = null;
            foreach (var item in s_LanguageImageKeys)
                LanguageImageKeys.Add(item);
            s_LanguageImageKeys = null;

            var set = new HashSet<SO_RSLocalizationTable>(PreloadLangeaueTable);
            PreloadLangeaueTable = set.Where(m => m != null).ToList();
        }
        private void OnEnable()
        {
            foreach (var type in TypeReader.FindAllTypesWhere(t =>
            {
                if (t.IsAbstract) return false;
                return t.IsSubclassOf(typeof(RSLocalizationKeyProvider));
            }))
            {
                RSLocalizationKeyProvider instance = (RSLocalizationKeyProvider)Activator.CreateInstance(type);
                foreach (var key in instance.TextKeys)
                    LanguageTextKeys.Add(key);
                foreach (var key in instance.ImageKeys)
                    LanguageImageKeys.Add(key);
            }
        }
    }
}
