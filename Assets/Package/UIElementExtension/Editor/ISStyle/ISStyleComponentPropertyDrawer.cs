using NaiveAPI.UITK;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NaiveAPI_Editor.UITK
{
    public abstract class RSStyleComponentPropertyDrawer<T> : PropertyDrawer
        where T : RSStyleComponent<T>, new()
    {
        public T value = new();
        public bool IsDirty = false;

        public bool IsExpanded = false;
        protected SerializedProperty OnGUIProperty;

        #region static & GenericMenu
        public Func<SerializedProperty> GetGenericMenuAffectProperty;
        public Action<SerializedProperty> GenericMenuSaveProperty;
        public RSStyleComponentPropertyDrawer()
        {
            GetGenericMenuAffectProperty = () => { return OnGUIProperty.Copy(); };
            GenericMenuSaveProperty = target => { SaveOn(target); };
        }
        static GenericMenu setUnsetMenu;
        static SerializedProperty setUnsetMenuProperty;
        static int setUnsetMenuFlag;
        static int setUnsetMenuMask;
        static RSStyleComponentPropertyDrawer<T> setUnsetMenuTarget;
        static T op_value = new();
        static RSStyleComponentPropertyDrawer()
        {
            setUnsetMenu = new GenericMenu();
            setUnsetMenu.AddItem(new GUIContent("Set"), false, () =>
            {
                setUnsetMenuTarget.value.LoadFrom(op_value);
                setUnsetMenuTarget.value.SetUnsetFlag = setUnsetMenuFlag;
                setUnsetMenuTarget.value.SetFlag(setUnsetMenuMask, true);
                setUnsetMenuTarget.GenericMenuSaveProperty(setUnsetMenuProperty);
            });
            setUnsetMenu.AddItem(new GUIContent("Unset"), false, () =>
            {
                setUnsetMenuTarget.value.LoadFrom(op_value);
                setUnsetMenuTarget.value.SetUnsetFlag = setUnsetMenuFlag;
                setUnsetMenuTarget.value.SetValueToDefault(setUnsetMenuMask);
                setUnsetMenuTarget.value.SetFlag(setUnsetMenuMask, false);
                setUnsetMenuTarget.GenericMenuSaveProperty(setUnsetMenuProperty);
            });
            setUnsetMenu.AddItem(new GUIContent("Set All"), false, () =>
            {
                setUnsetMenuTarget.value.LoadFrom(op_value);
                setUnsetMenuTarget.value.SetAll();
                setUnsetMenuTarget.GenericMenuSaveProperty(setUnsetMenuProperty);
            });
            setUnsetMenu.AddItem(new GUIContent("Unset All"), false, () =>
            {
                setUnsetMenuTarget.value.LoadFrom(op_value);
                setUnsetMenuTarget.value.SetValueToDefault(-1);
                setUnsetMenuTarget.value.SetUnsetFlag = 0;
                setUnsetMenuTarget.GenericMenuSaveProperty(setUnsetMenuProperty);
            });
        }
        void openSetUnsetMenu(int mask)
        {
            op_value.LoadFrom(value);
            setUnsetMenuProperty = GetGenericMenuAffectProperty();
            setUnsetMenuFlag = value.SetUnsetFlag;
            setUnsetMenuMask = mask;
            setUnsetMenuTarget = this;
            setUnsetMenu.ShowAsContext();
        }
        #endregion

        public abstract void DecodeValueFromProperty(SerializedProperty property);
        public abstract void EncodeValueToProperty(SerializedProperty property);
        protected virtual Rect BeforeRenderHeader(Rect headerRect, GUIContent label) { return headerRect; }
        public abstract int GetRenderHeight();
        /// <summary>
        /// RS Components implemented OnGUI
        /// </summary>
        public abstract void OnGUI(Rect position);
        /// <summary>
        /// OnGUI without using SerializedProperty
        /// </summary>
        public void OnGUI(Rect position, GUIContent label)
        {
            var orgIndent = EditorGUI.indentLevel;
            position.xMin += orgIndent * 18;
            EditorGUI.indentLevel = 0;
            Rect headerRect = position; headerRect.height = 18; headerRect.y += 1; headerRect.xMin -= 14;
            headerRect = BeforeRenderHeader(headerRect, label);
            Rect headerTopRect = headerRect; headerTopRect.height = 1;
            Rect headerBottomRect = headerRect; headerBottomRect.height = 1; headerBottomRect.y = headerRect.yMax - 1;
            Rect foldoutRect = headerRect; foldoutRect.xMin += 14;
            Rect contentRect = position; contentRect.yMin += 20;
            Rect contentColorRect = contentRect; contentColorRect.xMin -= 14;
            Color headerColor = new Color(.15f, .15f, .15f);
            Color headerLineColor = new Color(.3f, .3f, .3f);
            Color contentBackgroundColor = new Color(.19f, .19f, .19f);

            EditorGUI.DrawRect(headerRect, headerColor);
            EditorGUI.DrawRect(headerTopRect, headerLineColor);
            EditorGUI.DrawRect(headerBottomRect, headerLineColor);
            EditorGUI.DrawRect(contentColorRect, contentBackgroundColor);

            Color orgColor = GUI.color;
            GUI.color = Color.clear;
            IsExpanded = EditorGUI.BeginFoldoutHeaderGroup(foldoutRect, IsExpanded, label);
            GUI.color = orgColor;
            EditorGUI.Foldout(foldoutRect, IsExpanded, label);
            EditorGUI.EndFoldoutHeaderGroup();
            if (IsExpanded && (value != null))
            {
                OnGUI(contentRect);
            }
            EditorGUI.indentLevel = orgIndent;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            OnGUIProperty = property;
            IsExpanded = property.isExpanded;
            DecodeValueFromProperty(property);
            value.SetUnsetFlag = property.FindPropertyRelative("m_flag").intValue;

            OnGUI(position, label);

            property.isExpanded = IsExpanded;
            if (IsDirty)
                SaveOn(OnGUIProperty);
        }
        public float GetPropertyHeight()
        {
            if (IsExpanded)
                return GetRenderHeight() + 20;
            else
                return 20;
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            OnGUIProperty = property;
            if (property.isExpanded)
                return GetRenderHeight() + 20;
            else
                return 20;
        }
        public void SaveOn(SerializedProperty property)
        {
            EncodeValueToProperty(property);
            property.serializedObject.ApplyModifiedProperties();
            IsDirty = false;
        }

        Stack<int> SetUnsetFieldFlag = new();
        Stack<int> SetUnsetFieldMask = new();
        protected void BeginSetUnsetFieldByIndex(Rect rect, int index, int sizePx = 20)
        {
            BeginSetUnsetField(rect, 1 << index, sizePx);
        }
        protected void BeginSetUnsetField(Rect rect, int mask, int sizePx = 20)
        {
            SetUnsetFieldFlag.Push(value.SetUnsetFlag);
            SetUnsetFieldMask.Push(mask);
            EditorGUI.BeginChangeCheck();
            Event e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 1 && rect.Contains(e.mousePosition))
            {
                openSetUnsetMenu(mask);
            }
            bool val = value?.GetFlag(mask) ?? false;
            if (val)
            {
                rect.width = 3;
                rect.x -= 5;
                EditorGUI.DrawRect(rect, Color.gray);
            }
        }
        protected bool EndSetUnsetField()
        {
            bool isChange = EditorGUI.EndChangeCheck();
            value.SetUnsetFlag = SetUnsetFieldFlag.Peek();
            if (isChange)
            {
                value?.SetFlag(SetUnsetFieldMask.Peek(), true);
                IsDirty = true;
            }
            SetUnsetFieldFlag.Pop();
            SetUnsetFieldMask.Pop();
            return isChange;
        }
    }
}
