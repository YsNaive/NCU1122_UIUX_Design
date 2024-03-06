using NaiveAPI.DocumentBuilder;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NaiveAPI_Editor.RuntimeWindowUtils
{
    public class RuntimeWindowDocsWindow : EditorWindow
    {
        public SODocPage RootPage;

        [MenuItem("Tools/NaiveAPI/Runtime Window Docs", priority = 50)]
        #region DocBuilder Docs
        public static void ShowWindow()
        {
            GetWindow<RuntimeWindowDocsWindow>("Runtime Window docs");
        }
        #endregion
        private void CreateGUI()
        {
            rootVisualElement.Add(new DocBookVisual(RootPage) { DontPlayAnimation = true });
        }
    }
}
