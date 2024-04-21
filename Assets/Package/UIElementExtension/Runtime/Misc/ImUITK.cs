using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public static class IMUITK
    {
        static Stack<VisualElement> elementStack = new();
        static RSStyle m_style = new RSStyle { SetUnsetFlag = -1 };
        public static RSPosition Position => m_style.Position;
        public static RSMargin Margin => m_style.Margin;
        public static RSSize Size => m_style.Size;
        public static VisualElement Begin() { return Begin(new VisualElement()); }
        public static VisualElement Begin(VisualElement root)
        {
            if (elementStack.Count != 0)
                m_style.ApplyOn(elementStack.Peek());
            foreach (var item in m_style.VisitActiveStyle())
                item.UnsetAll();
            elementStack.Push(root);
            return root;
        }
        public static VisualElement BeginHorizontal(VisualElement root = null)
        {
            root ??= new VisualElement();
            root.style.flexDirection = FlexDirection.Row;
            return Begin(root);
        }
        public static VisualElement BeginVertical(VisualElement root = null)
        {
            root ??= new VisualElement();
            root.style.flexDirection = FlexDirection.Column;
            return Begin(root);
        }
        public static void End()
        {
            elementStack.Pop();
        }
        public static void End(VisualElement endTo)
        {

            while (elementStack.Peek() != endTo && elementStack.Count > 0)
                elementStack.Pop(); 
            elementStack.Pop();
        }
        public static VisualElement Finish()
        {
            while (elementStack.Count > 1)
                elementStack.Pop();
            return elementStack.Pop();
        }
    }
}
