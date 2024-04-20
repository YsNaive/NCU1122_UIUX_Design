using NaiveAPI.UITK;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static NaiveAPI.UITK.RSLocalization;

namespace NaiveAPI_Editor.UITK
{
    public class RSLocalizationSettingsGUIProvider : INaiveAPISettingsGUIProvider
    {
        public string MenuPath => "RS Localization";
        public int Priority => 1;

        public VisualElement CreateGUI()
        {
            VisualElement root = new RSScrollView();
            LanguageInfoListDrawer languageInfoListDrawer = new() { label = "Languages", value = UIElementExtensionResource.Get.LanguageInfos };
            languageInfoListDrawer.style.marginBottom = 4;
            root.Add(languageInfoListDrawer);

            LanguageKeysDrawer textKeysDrawer = new() { label = "Text Keys", value = UIElementExtensionResource.Get.LanguageTextKeys };
            textKeysDrawer.style.marginBottom = 4;
            root.Add(textKeysDrawer);

            LanguageKeysDrawer imageKeysDrawer = new() { label = "Image Keys", value = UIElementExtensionResource.Get.LanguageImageKeys };
            imageKeysDrawer.style.marginBottom = 4;
            root.Add(imageKeysDrawer);

            RSFoldout loadedTable = new RSFoldout() { text = "Loaded Table" };
            foreach (var item in LoadedTable)
                loadedTable.Add(new ObjectEditorDrawer { value = item });
            root.Add(loadedTable);
            root.Add(new RSButton("Reload", () =>
            {
                CurrentLanguage = "en";
                foreach (var ve in root.HierarchyRecursive<RSLocalizeTextElement>())
                    ve.ReloadText();
            }));
            root.Add(new RSButton("¨ê·s", () =>
            {
                CurrentLanguage = "zh-CHT";
                foreach (var ve in root.HierarchyRecursive<RSLocalizeTextElement>())
                    ve.ReloadText();
            }));
            return root;
        }

        class LanguageInfoDrawer : RuntimeDrawer<LanguageInfo>
        {
            PopupElement checkPopup;
            StringDrawer nameDrawer;
            ObjectEditorDrawer iconDrawer;
            public override void RepaintDrawer()
            {
                label = $" {value.key} : {value.name}";
                nameDrawer.SetValueWithoutNotify(value.name);
                iconDrawer.SetValueWithoutNotify(value.icon);
            }

            protected override void CreateGUI()
            {
                nameDrawer = new() { localizeLabel = new RSLocalizeText() { key = SR.name } };
                iconDrawer = new() { localizeLabel = new RSLocalizeText() { key = SR.icon } };
                iconDrawer.objectType = typeof(Sprite);
                nameDrawer.OnValueChanged += () =>
                {
                    value.name = nameDrawer.value;
                    label = $" {value.key} : {value.name}";
                };
                iconDrawer.OnValueChanged += () =>
                {
                    value.icon = (Sprite)iconDrawer.value;
                };
                Add(nameDrawer);
                Add(iconDrawer);

                checkPopup = new PopupElement();
                checkPopup.style.backgroundColor = RSTheme.Current.BackgroundColor;
                checkPopup.style.SetRS_Style(new RSBorder { anyColor = RSTheme.Current.FrontgroundColor, anyWidth = 1 });
                checkPopup.style.SetRS_Style(new RSPadding { any = 5 });
                RSButton deleteButton = new("Delete");
                deleteButton.style.marginLeft = StyleKeyword.Auto;
                deleteButton.style.left = StyleKeyword.Auto;
                deleteButton.clicked += () => checkPopup.OpenBelow(deleteButton);
                labelElement.Add(deleteButton);

                checkPopup.Add(new RSTextElement("Are you sure to Delete this language info?"));
                checkPopup.Add(new RSTextElement("It can <b>NOT</b> be undo."));
                checkPopup.Add(new RSButton("<b>Delete</b>", RSTheme.Current.DangerColorSet, () =>
                {
                    UIElementExtensionResource.Get.LanguageInfos.Remove(value);
                    EditorUtility.SetDirty(UIElementExtensionResource.Get);
                    parent?.Remove(this);
                    checkPopup.Close();
                })
                { style = { color = RSTheme.Current.DangerColorSet.TextColor } });
            }
        }
        class LanguageInfoListDrawer : FoldoutDrawer<List<LanguageInfo>>
        {
            public override void RepaintDrawer()
            {
                reloadContainer();
            }

            VisualElement container;
            protected override void CreateGUI()
            {
                RSHorizontal addNewKey = new();
                StringDrawer stringDrawer = new() { label = "Create New Key" };
                RSButton addNewKeyBtn = new("Add");
                addNewKeyBtn.SetEnabled(false);
                container = new();

                addNewKeyBtn.clicked += () =>
                {
                    value.Add(new() { key = stringDrawer.value });
                    EditorUtility.SetDirty(UIElementExtensionResource.Get);
                    stringDrawer.value = "";
                    reloadContainer();
                };
                stringDrawer.OnValueChanged += () =>
                {
                    if(stringDrawer.value.Contains(' '))
                    {
                        stringDrawer.value = stringDrawer.value.Replace(" ", "");
                        return;
                    }
                    addNewKeyBtn.SetEnabled(false);
                    if (string.IsNullOrEmpty(stringDrawer.value))
                        return;
                    if (value.Find(m => m.key == stringDrawer.value) != null)
                        return;
                    addNewKeyBtn.SetEnabled(true);
                };
                stringDrawer.value = "";
                stringDrawer.style.flexGrow = 1;
                addNewKeyBtn.style.width = 100;
                addNewKeyBtn.style.marginLeft = 10;
                addNewKey.style.flexShrink = 0;
                addNewKey.Add(stringDrawer);
                addNewKey.Add(addNewKeyBtn);

                Add(addNewKey);
                Add(container);
            }
            void reloadContainer()
            {
                var org = RSTheme.indentLevel;
                RSTheme.indentLevel = NestLevel + 1;
                container.Clear();
                foreach (var setting in value.OrderBy(m => m.key))
                {
                    var drawer = new LanguageInfoDrawer() { value = setting };
                    drawer.style.marginTop = 5;
                    drawer.LayoutExpand();
                    container.Add(drawer);
                }
                RSTheme.indentLevel = org;
            }
        }
        class LanguageKeysDrawer : FoldoutDrawer<HashSet<string>>
        {
            StringDrawer keyDrawer;
            IntegerDrawer maxDisplayDrawer;
            VisualElement displayContainer;
            List<string> matchedKeys = new();
            public override void RepaintDrawer()
            {
                reloadMatchedKeys();
                reloadDiaplayContainer();
            }

            protected override void CreateGUI()
            {
                displayContainer = new();
                displayContainer.style.paddingLeft = RSTheme.Current.IndentWidth;
                maxDisplayDrawer = new IntegerDrawer() {label = "DisplayCount" };
                maxDisplayDrawer.labelWidth = 80;
                maxDisplayDrawer.style.minWidth = 140;
                maxDisplayDrawer.style.marginLeft = StyleKeyword.Auto;
                maxDisplayDrawer.value = 20;
                maxDisplayDrawer.OnValueChanged += () =>
                {
                    reloadMatchedKeys();
                    reloadDiaplayContainer();
                };
                maxDisplayDrawer.RegisterCallback<PointerDownEvent>(evt => evt.StopImmediatePropagation());
                labelElement.Add(maxDisplayDrawer);

                RSButton addBtn = new RSButton("Add", RSTheme.Current.SuccessColorSet)
                {
                    style =
                    {
                        marginLeft = 4,
                        marginRight = StyleKeyword.Auto,
                        minWidth = 60
                    }
                };
                addBtn.clicked += () =>
                {
                    value.Add(keyDrawer.value);
                    EditorUtility.SetDirty(UIElementExtensionResource.Get);
                    addBtn.SetEnabled(false);
                    reloadMatchedKeys();
                    reloadDiaplayContainer();
                };
                addBtn.SetEnabled(false);

                keyDrawer = new StringDrawer() { label = "Key", multiline = false};
                keyDrawer.OnValueChanged += () =>
                {
                    reloadMatchedKeys();
                    reloadDiaplayContainer();
                    addBtn.SetEnabled((!string.IsNullOrEmpty(keyDrawer.value)) && (!value.Contains(keyDrawer.value)));
                };
                keyDrawer.hierarchy.Add(addBtn);

                Add(keyDrawer);
                Add(displayContainer);
            }
            void reloadMatchedKeys()
            {
                matchedKeys.Clear();
                int i = 0;
                foreach (string key in value.OrderBy(m =>
                {
                    var ret = m.LevenshteinDistance(keyDrawer.value);
                    if (m.Contains(keyDrawer.value, StringComparison.Ordinal))
                        ret -= 10;
                    return ret;
                })) { 
                    matchedKeys.Add(key);
                    if (i++ >= maxDisplayDrawer.value)
                        break;
                }
            }
            void reloadDiaplayContainer()
            {
                displayContainer.Clear();
                for (int i = 0; i < matchedKeys.Count; i++)
                {
                    var localI = i;
                    var hor = new RSHorizontal();
                    var deleteBtn = new RSButton("Delete",RSTheme.Current.DangerColorSet);
                    deleteBtn.style.marginRight = 6;
                    deleteBtn.clicked += () =>
                    {
                        value.Remove(matchedKeys[localI]);
                        EditorUtility.SetDirty(UIElementExtensionResource.Get);
                        matchedKeys.RemoveAt(localI);
                        reloadDiaplayContainer();
                    };
                    hor.Add(deleteBtn);
                    hor.Add(new RSTextElement(matchedKeys[i]) { style = { unityTextAlign = TextAnchor.MiddleLeft } });

                    displayContainer.Add(hor);
                }
            }
        }
    }
}
