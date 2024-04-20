using NaiveAPI.UITK;
using System;
using System.Collections.Generic;
using UnityEditor;
namespace NaiveAPI_Editor.UITK
{
    public class RSLocalizationKeyPickerWindow : EditorWindow
    {
        private void CreateGUI()
        {
            var root = rootVisualElement;
            root.style.SetRS_Style(new RSPadding { any = 6 });
            root.style.backgroundColor = RSTheme.Current.BackgroundColor;
            root.style.width = RSLength.Full;
            root.style.height = RSLength.Full;
        }
        public static void OpenNotDefinedTextKeyPicker(string languageKey,Action<string> callback)
        { openPicker(RSLocalization.FindNotDefinedTextKey(languageKey), callback); }
        public static void OpenTextKeyPicker(Action<string> callback)
        { openPicker(RSLocalization.TextKeys, callback); }
        public static void OpenNotDefinedSpriteKeyPicker(string languageKey, Action<string> callback)
        { openPicker(RSLocalization.FindNotDefinedSpriteKey(languageKey), callback); }
        public static void OpenSpriteKeyPicker(Action<string> callback)
        { openPicker(RSLocalization.SpriteKeys, callback); }
        static void openPicker(IEnumerable<string> choices, Action<string> callback)
        {
            var window = GetWindow<RSLocalizationKeyPickerWindow>("Pick Key");
            window.rootVisualElement.Clear();
            window.rootVisualElement.Add(RSLocalizationKeyPicker.GetPicker(choices, callback));
            RSLocalizationKeyPicker.callback += _ => window.Close();
        }
    }
}
