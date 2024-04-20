using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public static class ImUITK
    {
        static Stack<VisualElement> elementStack = new();

        public static void Begin(VisualElement root)
        {
            elementStack.Push(root);
        }
        public static VisualElement BeginHorizontal(VisualElement root = null)
        {
            root ??= new VisualElement();
            root.style.flexDirection = FlexDirection.Row;
            elementStack.Push(root);
            return root;
        }
        public static VisualElement BeginVertical(VisualElement root = null)
        {
            root ??= new VisualElement();
            root.style.flexDirection = FlexDirection.Column;
            elementStack.Push(root);
            return root;
        }
        public static void End()
        {
            elementStack.Pop();
        }
        public static void EndUntil(VisualElement endTo)
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
