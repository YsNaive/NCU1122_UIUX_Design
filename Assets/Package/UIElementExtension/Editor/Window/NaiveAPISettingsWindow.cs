using NaiveAPI.UITK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.UITK
{
    public interface INaiveAPISettingsGUIProvider
    {
        public string MenuPath { get; }
        public int Priority { get; }
        public VisualElement CreateGUI();
    }
    public class NaiveAPRSettingsWindow : EditorWindow
    {
        const string cachePath = "Editor/SettingsWindow.json";
        [Serializable]
        class Settings
        {
            public string CurrentCategory = "";
        }
        static Settings settings;
        public static void LoadCache()
        {
            var data = NaiveAPICache.Load(cachePath);
            settings = JsonUtility.FromJson<Settings>(data) ?? new();
        }
        public static void SaveCache()
        {
            var data = JsonUtility.ToJson(settings);
            NaiveAPICache.Save(cachePath, data);
        }

        [MenuItem("Tools/NaiveAPI/Settings", priority = 0)]
        static void _GetWindow()
        {
            GetWindow<NaiveAPRSettingsWindow>("NaiveAPI Settings");
        }

        static List<INaiveAPISettingsGUIProvider> ActiveProvider;
        static List<string> ActiveProviderPath;
        static PageView<string> SettingsView;
        static void Init()
        {
            LoadCache();
            if (ActiveProvider != null) return;
            ActiveProvider = TypeReader
                .FindAllTypesWhere(m => typeof(INaiveAPISettingsGUIProvider).IsAssignableFrom(m) && m.IsClass && !m.IsAbstract)
                .Select(m => Activator.CreateInstance(m) as INaiveAPISettingsGUIProvider)
                .OrderBy(m => m.Priority)
                .ToList();

            ActiveProviderPath = ActiveProvider
                .Select(m => m.MenuPath)
                .ToList();

            SettingsView = new();
            RSPadding.op_temp.any = RSTheme.Current.IndentStep;
            SettingsView.style.SetRS_Style(RSPadding.op_temp);
            for (int i = 0; i < ActiveProvider.Count; i++)
            {
                SettingsView.OpenOrCreatePage(ActiveProviderPath[i]);
                SettingsView.Add(ActiveProvider[i].CreateGUI());
            }
        }
        private void CreateGUI()
        {
            Init();
            var root = rootVisualElement;
            root.style.backgroundColor = RSTheme.Current.BackgroundColor;

            StringDropdownDrawer pathDropdown = new() { label = "Category" };
            pathDropdown.style.flexShrink = 0;
            pathDropdown.style.borderBottomWidth = 1.5f;
            pathDropdown.style.borderBottomColor = RSTheme.Current.BackgroundColor2;
            pathDropdown.SetChoices(ActiveProviderPath);
            pathDropdown.OnValueChanged += () =>
            {
                if (!SettingsView.TryOpenPage(pathDropdown.value))
                    settings.CurrentCategory = "";
                else
                    settings.CurrentCategory = pathDropdown.value;
            };
            pathDropdown.value = settings.CurrentCategory;

            root.Add(pathDropdown);
            root.Add(SettingsView);
        }
        private void OnDisable()
        {
            SaveCache();
        }
    }
}
