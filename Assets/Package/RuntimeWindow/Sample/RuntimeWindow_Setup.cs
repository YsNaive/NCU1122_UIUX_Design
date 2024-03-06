using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using NaiveAPI.RuntimeWindowUtils;
using NaiveAPI.DocumentBuilder;
using System;

namespace NaiveAPI.Sample
{
    class SampleWindow : DSRuntimeWindow
    {
        public SampleWindow()
        {
            MinSize = new Vector2(250, 200);
            Add(new DSTextElement("You can Drag / Resize this window"));
            Add(new DSTextElement("Modify and try properties on Inspector!"));
        }
    }

    public class RuntimeWindow_Setup : MonoBehaviour
    {
        public UIDocument UIDocument;

        private void Awake()
        {
            RuntimeWindow.ScreenElement = UIDocument.rootVisualElement;
        }
        IEnumerator Start()
        {
            var sampleWindow   = RuntimeWindow.GetWindow<SampleWindow>("Default Window");
            var inspector      = RuntimeWindow.GetWindow<RuntimeInspector>();
            var sceneHierarchy = RuntimeWindow.GetWindow<RuntimeSceneHierarchy>();
            inspector.LinkSceneHierarchy = true;
            inspector.Dragable = false;
            inspector.Resizable = false;
            inspector.PopupOnClick = false;
            sceneHierarchy.Dragable = false;
            sceneHierarchy.Resizable = false;
            sceneHierarchy.PopupOnClick = false;
            sampleWindow.Add(new DSButton("Select on Inspector", () => { inspector.Selecting = sampleWindow; }));
            sampleWindow.Add(RuntimeDrawer.Create(new SampleData()));
            yield return null;
            sampleWindow  .SetLayoutPercent(new Rect(0.4f, 0.1f, 0.25f, 0.4f));
            inspector     .SetLayoutPercentAnyway(RuntimeWindow.LayoutPercent.RightOneThird);
            sceneHierarchy.SetLayoutPercentAnyway(new Rect(0, 0, 0.2f, 1f));

        }
    }
}