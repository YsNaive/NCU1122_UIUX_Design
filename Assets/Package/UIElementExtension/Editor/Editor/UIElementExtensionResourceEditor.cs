using NaiveAPI.UITK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.UITK
{

    [CustomEditor(typeof(UIElementExtensionResource))]
    public class UIElementExtensionResourceEditor : Editor
    {
        RSScrollView root;
        VisualElement mainUI;
        VisualElement ignoreAsmEditUI;
        SerializedObject so;
        public override VisualElement CreateInspectorGUI()
        {


            so = new SerializedObject(target);
            root = new RSScrollView();
            root.style.SetRS_Style(new RSPadding { any = 10 });
            root.style.backgroundColor = RSTheme.Current.BackgroundColor;
            initMain();
            initIgnoreAsmEdit();
            root.Add(mainUI);
            return root;
        }

        void initMain()
        {
            mainUI = new VisualElement();

            var curStyle = so.FindProperty(nameof(UIElementExtensionResource.DefaultTheme));
            ObjectField objectField = new ObjectField();
            objectField.bindingPath = curStyle.propertyPath;
            objectField.Bind(serializedObject);
            //var curStyleField = DocEditor.NewObjectField<SO_RSTheme>("Current Style", evt => {
            //    curStyle.objectReferenceValue = evt.newValue;
            //    so.ApplyModifiedProperties();
            //});
            //curStyleField.value = curStyle.objectReferenceValue;
            mainUI.Add(objectField);

            var ignoreAsmHor = new RSHorizontal();
            var ignoreAsmLabel = new RSTextElement("Ignore Assembly");
            ignoreAsmLabel.style.width = RSTheme.Current.LabelWidth;
            var ignoreAsmBtn = new RSButton("Edit", () =>
            {
                root.Clear();
                root.Add(ignoreAsmEditUI);
            });
            ignoreAsmBtn.style.flexGrow = 1;
            ignoreAsmHor.Add(ignoreAsmLabel);
            ignoreAsmHor.Add(ignoreAsmBtn);
            mainUI.Add(ignoreAsmHor);
        }
        void initIgnoreAsmEdit()
        {
            var ignoreAsm = so.FindProperty("m_IgnoreAssemblyName");
            ignoreAsmEditUI = new VisualElement();
            var backBtn = new RSButton("Back", () =>
            {
                root.Clear();
                root.Add(mainUI);
            });
            ignoreAsmEditUI.Add(backBtn);
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies().OrderBy(asm => { return asm.GetName().Name; }))
            {
                var name = asm.GetName().Name;
                var isIgnore = false;
                for (int i = 0, imax = ignoreAsm.arraySize; i < imax; i++)
                {
                    if (name == ignoreAsm.GetArrayElementAtIndex(i).stringValue)
                    {
                        isIgnore = true;
                        break;
                    }
                }

                var toggle = new BoolDrawer() { label = name, frontBox = true };
                toggle.value = isIgnore;
                toggle.OnValueChanged += () =>
                {
                    if (toggle.value)
                    {
                        var i = ignoreAsm.arraySize;
                        ignoreAsm.InsertArrayElementAtIndex(i);
                        ignoreAsm.GetArrayElementAtIndex(i).stringValue = name;
                    }
                    else
                    {
                        for (int i = 0, imax = ignoreAsm.arraySize; i < imax; i++)
                        {
                            if (name == ignoreAsm.GetArrayElementAtIndex(i).stringValue)
                            {
                                ignoreAsm.DeleteArrayElementAtIndex(i);
                                break;
                            }
                        }
                    }
                    so.ApplyModifiedProperties();
                };
                ignoreAsmEditUI.Add(toggle);
            }
        }
    }
}
