using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using NaiveAPI.UITK;
using System;

namespace NaiveAPI.Sample
{
    class SampleWindow : RSRuntimeWindow
    {
        public SampleWindow()
        {
            ContextMenu.Add("Close Window", Close);
            ContextMenu.Add("Bring to front", BringToFront);
            ContextMenu.Add("Send to back", SendToBack);

            InitLayoutAsPercent(new Rect(0.4f, 0.1f, 0.25f, 0.4f));
            Add(new RSTextElement("You can Drag / Resize this window"));
            Add(new RSTextElement("Modify and try properties on Inspector!"));
        }
    }

    public class RuntimeWindow_Setup : MonoBehaviour
    {
        public UIDocument UIDocument;

        private void Awake()
        {
            RuntimeWindow.ScreenElement = UIDocument.rootVisualElement;

        }
        void Start()
        {
            var sampleWindow   = RuntimeWindow.GetWindow<SampleWindow>("Default Window");
            var inspector      = RuntimeWindow.GetWindow<RuntimeInspector>();
            var sceneHierarchy = RuntimeWindow.GetWindow<RuntimeSceneHierarchy>();
            inspector.LinkSceneHierarchy = true;
            inspector.Dragable = false;
            inspector.Resizable = false;
            inspector.PopupOnClick = false;
            inspector.LimitSize = false;
            sceneHierarchy.Dragable = false;
            sceneHierarchy.Resizable = false;
            sceneHierarchy.PopupOnClick = false;
            sceneHierarchy.LimitSize = false;

            sampleWindow.Add(new RSButton("Select on Inspector", () => { inspector.Selecting = sampleWindow; }));
            sampleWindow.Add(RuntimeDrawer.Create(new RSColorSet(), "ColorSet"));
            inspector     .InitLayoutAsPercent(RuntimeWindow.LayoutPercent.RightOneThird);
            sceneHierarchy.InitLayoutAsPercent(new Rect(0, 0, 0.2f, 1f));

            //yield return 0;
            //inspector.SetLayoutInPercent(RuntimeWindow.LayoutPercent.RightOneThird);
            //sceneHierarchy.SetLayoutInPercent(new Rect(0, 0, 0.2f, 1f));
        }
    }
}