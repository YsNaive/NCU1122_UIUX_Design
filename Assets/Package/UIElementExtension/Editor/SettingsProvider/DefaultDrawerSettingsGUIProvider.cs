using NaiveAPI.UITK;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.UITK
{
    public class DefaultDrawerSettingsGUIProvider : INaiveAPISettingsGUIProvider
    {
        public string MenuPath => "DefaultDrawer Layout Preferences";

        public int Priority => 0;

        public VisualElement CreateGUI()
        {
            var root = new VisualElement();

            TypeDrawer typeDropdown = new() { label = "Value Type" };
            typeDropdown.style.flexShrink = 0;
            root.Add(typeDropdown);

            var container = new RSScrollView();
            root.Add(container);
            typeDropdown.OnValueChanged += () =>
            {
                container.Clear();
                var drawerType = RuntimeDrawer.FindDrawerType(typeDropdown.value);
                if(drawerType != typeof(DefaultDrawer))
                {
                    RSHorizontal hor = new();
                    hor.Add(new RSTextElement("This Type already defined a Drawer :") { style = { unityTextAlign = TextAnchor.MiddleLeft } });
                    hor.Add(new RSTypeNameElement(drawerType) { style = { unityTextAlign = TextAnchor.MiddleLeft } });
                    container.Add(hor);
                }

                var settings = UIElementExtensionResource.Get_or_CreateDefaultDrawerSettings(typeDropdown.value);

                ObjectEditorDrawer iconField = new() { label = "Icon" };
                iconField.objectType = typeof(Sprite);
                iconField.value = settings.Icon;
                iconField.OnValueChanged += () =>
                {
                    settings.Icon = iconField.value as Sprite;
                    EditorUtility.SetDirty(UIElementExtensionResource.Get);
                };
                container.Add(iconField);

                BoolDrawer affectDerived = new() { label = "Affect Derived Type" };
                affectDerived.style.marginBottom = RSTheme.Current.LineHeight / 2f;
                affectDerived.value = settings.AffectOnDerived;
                affectDerived.OnValueChanged += () =>
                {
                    settings.AffectOnDerived = affectDerived.value;
                    EditorUtility.SetDirty(UIElementExtensionResource.Get);
                };
                container.Add(affectDerived);


                foreach(var memberInfo in DefaultDrawer.GetExposeableMember(typeDropdown.value))
                {
                    var curInfo = memberInfo;
                    BoolDrawer toggle = new() {frontBox = true , label = memberInfo.Name};
                    toggle.iconElement.style.backgroundImage = Background.FromSprite((memberInfo is FieldInfo)? RSTheme.Current.CSharp.fieldIcon : RSTheme.Current.CSharp.propertyIcon);
                    bool defaultState = DefaultDrawer.DefaultMemberFilter(curInfo);
                    if ((defaultState ? settings.HideMember : settings.ExposeMember).Contains(memberInfo.Name))
                        toggle.SetValueWithoutNotify(!defaultState);
                    else
                        toggle.SetValueWithoutNotify(defaultState);
                    toggle.hierarchy.Add(new RSTypeNameElement(memberInfo.FieldOrPropertyType()) { style = {unityTextAlign = TextAnchor.MiddleLeft}});
                    toggle.OnValueChanged += () =>
                    {
                        if (toggle.value)
                        {
                            if(defaultState)
                                settings.HideMember.Remove(memberInfo.Name);
                            else
                                settings.ExposeMember.Add(memberInfo.Name);
                        }
                        else
                        {
                            if (defaultState)
                                settings.HideMember.Add(memberInfo.Name);
                            else
                                settings.ExposeMember.Remove(memberInfo.Name);
                        }
                        EditorUtility.SetDirty(UIElementExtensionResource.Get);
                    };
                    container.Add(toggle);
                }
            };

            return root;
        }
    }
}
