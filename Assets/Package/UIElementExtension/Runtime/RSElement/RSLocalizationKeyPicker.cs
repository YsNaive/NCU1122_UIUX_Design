using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public static class RSLocalizationKeyPicker
    {
        static VisualElement root;
        static StringDrawer searchDrawer;
        static RSScrollView choicesContainer;
        static IEnumerable<string> choices;
        public static int MaxDisplayCount = 50;
        public static event Action<string> callback;
        static RSLocalizationKeyPicker()
        {
            root = new VisualElement();
            root.style.SetRS_Style(new RSPadding { any = 6 });
            root.style.backgroundColor = RSTheme.Current.BackgroundColor;
            root.style.width = RSLength.Full;
            root.style.height = RSLength.Full;
            choicesContainer = new();
            choicesContainer.style.flexShrink = 1;
            searchDrawer = new StringDrawer() { label = "Search", style = { flexShrink = 0 } };
            searchDrawer.labelWidth = 80;
            searchDrawer.OnValueChanged += reloadChoices;
            searchDrawer.style.marginBottom = 4;
            root.Add(searchDrawer);
            root.Add(choicesContainer);
        }
        static void reloadChoices()
        {
            choicesContainer.Clear();
            int i = 0;
            foreach (var str in choices.OrderBy(m => m.LevenshteinDistance(searchDrawer.value)))
            {
                var localStr = str;
                var choice = new RSButton(str);
                choice.style.marginLeft = RSTheme.Current.LineHeight;
                choice.style.unityTextAlign = TextAnchor.MiddleLeft;
                choice.style.marginBottom = 2;
                choice.clicked += () =>
                {
                    callback?.Invoke(localStr);
                    callback = null;
                };
                choicesContainer.Add(choice);
                if (i++ > MaxDisplayCount)
                    break;
            }
        }
        public static VisualElement GetNotDefinedTextKeyPicker(string languageKey, Action<string> callback)
        { return GetPicker(RSLocalization.FindNotDefinedTextKey(languageKey), callback); }
        public static VisualElement GetTextKeyPicker(Action<string> callback)
        { return GetPicker(RSLocalization.TextKeys, callback); }
        public static VisualElement GetNotDefinedSpriteKeyPicker(string languageKey, Action<string> callback)
        { return GetPicker(RSLocalization.FindNotDefinedSpriteKey(languageKey), callback); }
        public static VisualElement GetSpriteKeyPicker(Action<string> callback)
        { return GetPicker(RSLocalization.SpriteKeys, callback); }
        public static VisualElement GetPicker(IEnumerable<string> choices, Action<string> callback)
        {
            RSLocalizationKeyPicker.choices = choices;
            reloadChoices();
            RSLocalizationKeyPicker.callback += callback;
            return root;
        }
    }
}
