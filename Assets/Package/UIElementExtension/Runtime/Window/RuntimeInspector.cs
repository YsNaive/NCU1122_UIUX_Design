using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public class RuntimeInspector : RSRuntimeWindow
    {

        public bool LinkSceneHierarchy { get; set; } = true;
        public bool ShowProperties
        {
            get => properties.style.display == DisplayStyle.Flex;
            set => properties.style.display = properties.style.display == DisplayStyle.Flex ? DisplayStyle.None : DisplayStyle.Flex;
        }
        public bool UpdateField
        {
            get => m_UpdateField;
            set
            {
                m_UpdateField = value;
                if(value)
                    m_UpdateItem.Resume();
                else
                    m_UpdateItem.Pause();
            }
        }
        public bool m_UpdateField  = true;
        public int UpdateDelay
        {
            get => m_UpdateDelay;
            set
            {
                m_UpdateDelay = value;
                m_UpdateItem.Every(value);
            }
        }
        private int m_UpdateDelay = 250;
        IVisualElementScheduledItem m_UpdateItem;
        public object Selecting
        {
            get => m_Selecting;
            set
            {
                if (m_Selecting == value) return;
                m_Selecting = value;
                ReGenerateDrawer();
            }
        }
        private object m_Selecting;
        public RuntimeDrawer CurrentDrawer => m_CurrentDrawer;
        private RuntimeDrawer m_CurrentDrawer;
        private RSScrollView scrollView;
        private RSHorizontal header;
        private VisualElement properties;
        public RuntimeInspector()
        {
            RuntimeSceneHierarchy.OnSelectObject += obj => { Selecting = obj; };
            scrollView = new RSScrollView();
            header = new RSHorizontal()
            {
                style =
                {
                    flexShrink = 0,
                }
            };
            properties = new VisualElement();
            properties.style.flexShrink = 0;
            IntegerDrawer updateDelay = new IntegerDrawer() { value = m_UpdateDelay, label = "Update Delay" };
            updateDelay.OnValueChanged += () => { UpdateDelay = updateDelay.value; };
            BoolDrawer isUpdate = new BoolDrawer() { value = true, label = "Update Field" };
            isUpdate.OnValueChanged += () => { UpdateField = isUpdate.value; updateDelay.SetEnabled(isUpdate.value); };
            RSTextElement updateLoop = new RSTextElement("Update Loop : ");
            updateLoop.style.marginLeft = RSTheme.Current.LineHeight;
            properties.Add(isUpdate);
            properties.Add(updateDelay);
            properties.Add(updateLoop);
            properties.style.display = DisplayStyle.None;
            Add(properties);
            Add(header);
            Add(scrollView);
            TabName = "Inspector";

            m_UpdateItem = schedule.Execute(() => {
                var begTime = Time.realtimeSinceStartup;
                m_CurrentDrawer?.RepaintDrawer();
                var costTime = Time.realtimeSinceStartup - begTime;
                ((INotifyValueChanged<string>)updateLoop)
                .SetValueWithoutNotify($"Update Loop Time : {costTime * 1000:000.00} ms");

            }).Every(m_UpdateDelay);

            ContextMenu.Add("Properties", () => { ShowProperties = !ShowProperties; });
        }
        public void ReGenerateDrawer()
        {
            scrollView.Clear();
            var typeView = header.Q<RSTypeNameElement>();
            typeView?.parent.Remove(typeView);
            m_CurrentDrawer = null;
            if (m_Selecting == null) return;
            header.Insert(0, new RSTypeNameElement(m_Selecting.GetType()));
            m_CurrentDrawer = m_Selecting switch
            {
                GameObject obj => new GameObjectDrawer() { value = obj },
                _ => RuntimeDrawer.Create(m_Selecting, TypeReader.GetName(m_Selecting.GetType()))
            };
            m_CurrentDrawer.style.marginBottom = RSTheme.Current.LineHeight * 10;
            IFoldoutDrawer asFoldout = m_CurrentDrawer as IFoldoutDrawer;
            if (asFoldout != null) asFoldout.DisFoldout();
            //(m_CurrentDrawer as UnityObjectDrawer)?.LayoutExpand();

            scrollView.Add(m_CurrentDrawer);
        }
    }
}