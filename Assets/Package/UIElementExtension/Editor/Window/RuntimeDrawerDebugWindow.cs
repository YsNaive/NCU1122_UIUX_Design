using NaiveAPI.UITK;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.UITK
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
            rootVisualElement.style.backgroundColor = RSTheme.Current.BackgroundColor;
            rootVisualElement.style.SetRS_Style(new RSPadding { any = 5 });

            GridView gridView = new GridView(RuntimeDrawer.ActiveDrawer.Count() + 1, 6, RSTheme.Current.FrontgroundColor, GridView.AlignMode.FixedContent);
            gridView[0, 0].Add(new RSTextElement("Drawer Type"));
            gridView[0, 1].Add(new RSTextElement("Data Type"));
            gridView[0, 2].Add(new RSTextElement("Pr"));
            gridView[0, 3].Add(new RSTextElement("D"));
            gridView[0, 4].Add(new RSTextElement("A"));
            gridView[0, 5].Add(new RSTextElement("Require Attr"));
            int i = 1;
            foreach(var drawer in RuntimeDrawer.ActiveDrawer)
            {
                gridView[i, 0].Add(new RSTypeNameElement(drawer.drawerType));
                gridView[i, 1].Add(new RSTypeNameElement(drawer.attribute.TargetType));
                gridView[i, 2].Add(new RSTextElement(drawer.attribute.Priority.ToString()));
                gridView[i, 3].Add(new RSTextElement(drawer.attribute.DrawDerivedType? "―" : "ー"));
                gridView[i, 4].Add(new RSTextElement(drawer.attribute.DrawAssignableType? "―" : "ー"));
                if(drawer.attribute.RequiredAttribute != null)
                    gridView[i, 5].Add(new RSTypeNameElement(drawer.attribute.RequiredAttribute));
                i++;
            }

            //var typeField = new DSTypeField("Find Match");
            //typeField.style.flexShrink = 0;
            //var matchContainer = new VisualElement();
            //matchContainer.style.flexShrink = 0;
            //matchContainer.style.marginBottom = 8;
            //matchContainer.Add(new RSTextElement("N/A"));
            //typeField.RegisterValueChangedCallback(evt =>
            //{
            //    matchContainer.Clear();
            //    for(int i=1; i< gridView.Row; i++)
            //        gridView[i, 0].style.backgroundColor = DocStyle.Current.BackgroundColor;
            //    if (evt.newValue == null)
            //    {
            //        matchContainer.Add(new RSTextElement("N/A"));
            //        return;
            //    }
            //    else
            //    {
            //        var matched = RuntimeDrawer.FindDrawerType(evt.newValue);
            //        for (int i = 1; i < gridView.Row; i++)
            //        {
            //            if (matched == gridView[i, 0].Q<DSTypeNameElement>().TargetType)
            //            {
            //                gridView[i, 0].style.backgroundColor = DocStyle.Current.SuccessColor;
            //                break;
            //            }
            //        }
            //        matchContainer.Add(new DSTypeNameElement(matched));
            //    }
            //});
            //rootVisualElement.Add(typeField);
            //rootVisualElement.Add(matchContainer);

            RSScrollView sc = new RSScrollView();
            sc.Add(gridView);
            rootVisualElement.Add(sc);
        }
    }
}
