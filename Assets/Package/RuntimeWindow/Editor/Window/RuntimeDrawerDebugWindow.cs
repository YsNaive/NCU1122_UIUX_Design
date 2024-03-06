using NaiveAPI.DocumentBuilder;
using NaiveAPI.RuntimeWindowUtils;
using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.RuntimeWindow
{
    public class RuntimeDrawerDebugWindow : EditorWindow
    {
        [MenuItem("Tools/NaiveAPI/RuntimeDrawer Debug", priority = 51)]
        public static void Open()
        {
            GetWindow<RuntimeDrawerDebugWindow>("RuntimeDrawer Debug");
        }
        private void CreateGUI()
        {
            rootVisualElement.style.backgroundColor = DocStyle.Current.BackgroundColor;
            rootVisualElement.style.SetIS_Style(ISPadding.Pixel(5));

            GridView gridView = new GridView(RuntimeDrawer.ActiveDrawer.Count() + 1, 6, DocStyle.Current.SubBackgroundColor, GridView.AlignMode.FixedContent);
            gridView[0, 0].Add(new DSTextElement("Drawer Type"));
            gridView[0, 1].Add(new DSTextElement("Data Type"));
            gridView[0, 2].Add(new DSTextElement("Pr"));
            gridView[0, 3].Add(new DSTextElement("D"));
            gridView[0, 4].Add(new DSTextElement("A"));
            gridView[0, 5].Add(new DSTextElement("Require Attr"));
            int i = 1;
            foreach(var drawer in RuntimeDrawer.ActiveDrawer)
            {
                gridView[i, 0].Add(new DSTypeNameElement(drawer.drawerType));
                gridView[i, 1].Add(new DSTypeNameElement(drawer.attribute.TargetType));
                gridView[i, 2].Add(new DSTextElement(drawer.attribute.Priority.ToString()));
                gridView[i, 3].Add(new DSTextElement(drawer.attribute.DrawDerivedType? "―" : "ー"));
                gridView[i, 4].Add(new DSTextElement(drawer.attribute.DrawAssignableType? "―" : "ー"));
                if(drawer.attribute.RequiredAttribute != null)
                    gridView[i, 5].Add(new DSTypeNameElement(drawer.attribute.RequiredAttribute));
                i++;
            }

            var typeField = new DSTypeField("Find Match");
            typeField.style.flexShrink = 0;
            var matchContainer = new VisualElement();
            matchContainer.style.flexShrink = 0;
            matchContainer.style.marginBottom = 8;
            matchContainer.Add(new DSTextElement("N/A"));
            typeField.RegisterValueChangedCallback(evt =>
            {
                matchContainer.Clear();
                for(int i=1; i< gridView.Row; i++)
                    gridView[i, 0].style.backgroundColor = DocStyle.Current.BackgroundColor;
                if (evt.newValue == null)
                {
                    matchContainer.Add(new DSTextElement("N/A"));
                    return;
                }
                else
                {
                    var matched = RuntimeDrawer.FindDrawerType(evt.newValue);
                    for (int i = 1; i < gridView.Row; i++)
                    {
                        if (matched == gridView[i, 0].Q<DSTypeNameElement>().TargetType)
                        {
                            gridView[i, 0].style.backgroundColor = DocStyle.Current.SuccessColor;
                            break;
                        }
                    }
                    matchContainer.Add(new DSTypeNameElement(matched));
                }
            });
            rootVisualElement.Add(typeField);
            rootVisualElement.Add(matchContainer);

            DSScrollView sc = new DSScrollView();
            sc.Add(gridView);
            rootVisualElement.Add(sc);
        }
    }
}
