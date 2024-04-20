using NaiveAPI.UITK;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.UITK
{

    public class SO_RSLocalizationTablePostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (var assetPath in importedAssets)
            {
                var asset = AssetDatabase.LoadAssetAtPath<SO_RSLocalizationTable>(assetPath);
                if (asset != null)
                {
                    UIElementExtensionResource.Get.PreloadLangeaueTable.Add(asset);
                    EditorUtility.SetDirty(UIElementExtensionResource.Get);
                }
            }
        }
    }
    [CustomEditor(typeof(SO_RSLocalizationTable))]
    public class SO_RSLocalizationTableEditor : Editor
    {
        static readonly string[] Mode = new string[] { "Text", "Sprite" };
        VisualElement root;
        SO_RSLocalizationTable value;
        Dictionary<string, VisualElement> editDrawerTable = new();
        RSScrollView editViewContainer;
        IntegerDrawer maxDisplay;
        StringDropdownDrawer modeDrawer;
        StringDrawer searchDrawer;
        Action openPicker;
        Action<string> addToTable;
        Action<string> removeFromTable;
        public override VisualElement CreateInspectorGUI()
        {
            value = (SO_RSLocalizationTable)target;
            root = new();
            editViewContainer = new();
            root.style.SetRS_Style(new RSPadding { any = 5 });
            root.style.backgroundColor = RSTheme.Current.BackgroundColor;
            root.style.minHeight = 400;
            StringDropdownDrawer languageKeyDropdown = new StringDropdownDrawer() { label = "Language" };
            languageKeyDropdown.SetChoices(UIElementExtensionResource.Get.LanguageKeys);
            languageKeyDropdown.value = value.LanguageKey;
            languageKeyDropdown.OnValueChanged += () =>
            {
                value.LanguageKey = languageKeyDropdown.value;
                EditorUtility.SetDirty(this);
            };
            root.Add(languageKeyDropdown);

            ObjectEditorDrawer fontAssetDrawer = new ObjectEditorDrawer()
            {
                label = "Font Asset",
                value = value.FontAsset,
                objectType = typeof(FontAsset),
                style = {marginBottom = 6}
            };
            fontAssetDrawer.OnValueChanged += () =>
            {
                value.FontAsset = (FontAsset)fontAssetDrawer.value;
                EditorUtility.SetDirty(this);
            };
            root.Add(fontAssetDrawer);
            maxDisplay = new IntegerDrawer { label = "Display Count", value = 50, labelWidth = 100 };
            modeDrawer = new StringDropdownDrawer { label = "Mode", labelWidth = 80};
            modeDrawer.SetChoices(Mode);
            modeDrawer.OnValueChanged += () =>
            {
                reloadDrawerTable();
                reloadEditDisplay();
                if (modeDrawer.value == Mode[0])
                {
                    openPicker = () =>
                    {
                        RSLocalizationKeyPickerWindow.OpenNotDefinedTextKeyPicker(value.LanguageKey, (key) =>
                        { searchDrawer.value = key; });
                    };
                    addToTable = (val) =>
                    {
                        value.TextTable.Add(val, "");
                    };
                    removeFromTable = (val) =>
                    {
                        value.TextTable.Remove(val);
                    };
                }
                else
                {
                    openPicker = () =>
                    {
                        RSLocalizationKeyPickerWindow.OpenNotDefinedSpriteKeyPicker(value.LanguageKey, (key) =>
                        { searchDrawer.value = key; });
                    };
                    addToTable = (val) =>
                    {
                        value.SpriteTable.Add(val, null);
                    };
                    removeFromTable = (val) =>
                    {
                        value.SpriteTable.Remove(val);
                    };
                }
            };
            searchDrawer = new StringDrawer
            {
                label = "Search Key",
                multiline = false,
                style = {marginBottom = 6},
                labelWidth = 80
            };
            RSButton addTextBtn = new RSButton("Add", RSTheme.Current.SuccessColorSet);
            RSButton pickBtn = new RSButton("Pick not Defined", RSTheme.Current.HintColorSet);
            addTextBtn.SetEnabled(false);
            addTextBtn.style.marginRight = StyleKeyword.Auto;
            pickBtn.style.marginRight = StyleKeyword.Auto;
            pickBtn.clicked += () =>
            {
                openPicker();
            };
            searchDrawer.hierarchy.Add(pickBtn);
            searchDrawer.hierarchy.Add(addTextBtn);
            searchDrawer.OnValueChanged += () =>
            {
                reloadEditDisplay();
                addTextBtn.SetEnabled(!string.IsNullOrEmpty(searchDrawer.value) && !value.TextTable.ContainsKey(searchDrawer.value));
            };
            addTextBtn.clicked += () =>
            {
                addTextBtn.SetEnabled(false);
                addToTable(searchDrawer.value);
                reloadDrawerTable();
                reloadEditDisplay();
            };
            root.Add(new RSHorizontal( modeDrawer, maxDisplay));
            root.Add(searchDrawer);
            root.Add(editViewContainer);

            modeDrawer.value = Mode[0];
            return root;
        }

        void reloadEditDisplay()
        {
            editViewContainer.Clear();
            int i = -1;
            foreach (var pair in editDrawerTable.OrderBy(pair => pair.Key.LevenshteinDistanceBasedCost(searchDrawer.value)))
            {
                editViewContainer.Add(pair.Value);
                if (i++ >= maxDisplay.value)
                    break;
            }
        }
        void reloadDrawerTable()
        {
            editDrawerTable.Clear();
            editViewContainer.Clear();
            Action<RuntimeDrawer, string> addDeleteBtn = (drawer, localKey) =>
            {
                RSButton deleteBtn = new RSButton("Delete", RSTheme.Current.DangerColorSet);
                deleteBtn.style.marginRight = StyleKeyword.Auto;
                deleteBtn.style.marginLeft = 4;
                drawer.hierarchy.Add(deleteBtn);
                deleteBtn.clicked += () =>
                {
                    removeFromTable(localKey);
                    editDrawerTable.Remove(localKey);
                    reloadEditDisplay();
                    EditorUtility.SetDirty(value);
                };
                editDrawerTable.Add(localKey, drawer);
            };
            if(modeDrawer.value == Mode[0])
            {
                foreach (var pair in value.TextTable)
                {
                    var localKey = pair.Key;
                    StringDrawer drawer = new StringDrawer()
                    {
                        label = localKey,
                        value = pair.Value,
                    };
                    drawer.value = pair.Value;
                    drawer.OnValueChanged += () =>
                    {
                        value.TextTable[localKey] = drawer.value;
                        EditorUtility.SetDirty(value);
                    };
                    addDeleteBtn(drawer, localKey);
                }
            }
            else
            {
                foreach (var pair in value.SpriteTable)
                {
                    var localKey = pair.Key;
                    ObjectEditorDrawer drawer = new ObjectEditorDrawer()
                    {
                        label = localKey,
                        value = pair.Value,
                        objectType = typeof(Sprite),
                    };
                    drawer.value = pair.Value;
                    drawer.OnValueChanged += () =>
                    {
                        value.SpriteTable[localKey] = (Sprite)drawer.value;
                        EditorUtility.SetDirty(value);
                    };
                    addDeleteBtn(drawer, localKey);
                }
            }
        }
    }
}
