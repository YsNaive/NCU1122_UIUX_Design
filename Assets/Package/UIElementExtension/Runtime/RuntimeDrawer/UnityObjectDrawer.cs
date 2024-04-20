using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace NaiveAPI.UITK
{
    [CustomRuntimeDrawer(typeof(Object), DrawDerivedType = true, Priority = -1)]
    public class UnityObjectDrawer : UnityObjectDrawer<Object>
    {
        public override bool DynamicLayout => true;

        public override Type objectType => m_objectType;
        private Type m_objectType;
        public void SetObjectType(Type type)
        {
            m_objectType = type;
            value = null;
        }
        protected override void CreateGUI()
        {
            Clear();
            if(value!=null) m_objectType = value.GetType();
            SetIcon(DefaultDrawer.GetDefaultIcon(objectType));
            if (objectType == null) return;
            foreach (var member in DefaultDrawer.GetExposeMember(objectType))
            {
                switch (member)
                {
                    case FieldInfo fieldInfo:
                        AddDrawer(fieldInfo.Name, fieldInfo.FieldType, () => fieldInfo.GetValue(value), (v) => fieldInfo.SetValue(value, v));
                        break;
                    case PropertyInfo propertyInfo:
                        AddDrawer(propertyInfo.Name, propertyInfo.PropertyType, () => propertyInfo.GetValue(value), (v) => propertyInfo.SetValue(value, v));
                        break;
                }
            }
        }
    }
    public abstract class UnityObjectDrawer<T> : StandardDrawer<T>
        where T : Object
    {
        public virtual Type objectType => typeof(T);

        Dictionary<string, Object> choiceDict = new();
        RSTextElement objRefPicker;
        bool m_isDrawRefence = true;
        public UnityObjectDrawer()
        {
            objRefPicker = new RSTextElement();
            objRefPicker.style.maxHeight = RSTheme.Current.LineHeight;
            objRefPicker.style.whiteSpace = WhiteSpace.NoWrap;
            objRefPicker.style.SetRS_Style(RSTheme.Current.FieldStyle);
            objRefPicker.style.paddingRight = 0;
            objRefPicker.style.flexGrow = 1;
            (objRefPicker as INotifyValueChanged<string>).SetValueWithoutNotify("None");
            objRefPicker.RegisterCallback<PointerDownEvent>(evt =>
            {
                reloadChoice();
                var popup = RSContextMenu.CreatePopupMenu(choiceDict.Keys.ToList(),
                                  selected => { SetValue(choiceDict[selected]); });
                popup.OpenBelow(objRefPicker);
                evt.StopPropagation();
            });
            objRefPicker.Add(createObjRefIcon());
            labelElement.Add(objRefPicker);
            if (RSTheme.indentLevel != 0)
            {
                m_isDrawRefence = false;
                LayoutInline();
            }
            else
            {
                m_isDrawRefence = true;
                LayoutExpand();
            }
        }
        void reloadChoice()
        {
            choiceDict.Clear();
            if (objectType == null) return;
            foreach (var obj in Object.FindObjectsByType(objectType, FindObjectsSortMode.None))
            {
                var orgName = obj.name;
                var name = obj.name;
                int i = 2;
                while (choiceDict.ContainsKey(name))
                    name = $"{orgName}_{i++}";
                choiceDict.Add(name, obj);
            }
        }
        VisualElement createObjRefIcon()
        {
            var radius = RSRadius.Percent(75);
            var size = RSTheme.Current.LineHeight;
            var scale = new Vector3(.9f, .9f, 1f);
            var circle0 = new VisualElement()
            {
                style = {
                    width = size,
                    height= size,
                    backgroundColor = new Color(.22f,.22f,.22f),
                    marginLeft = StyleKeyword.Auto,
                    marginRight = 0,
                    borderLeftWidth = 1,
                    borderLeftColor = new Color(.5f,.5f,.5f),
                    alignContent = Align.Center,
                    justifyContent = Justify.Center,
                }
            };
            var circle1 = new VisualElement()
            {
                style = {
                    width = size,
                    height= size,
                    backgroundColor = new Color(.8f,.8f,.8f),
                    alignContent = Align.Center,
                    justifyContent = Justify.Center,
                }
            };
            var circle2 = new VisualElement()
            {
                style = {
                    width = size,
                    height= size,
                    backgroundColor = new Color(.22f,.22f,.22f),
                    alignContent = Align.Center,
                    justifyContent = Justify.Center,
                }
            };
            var circle3 = new VisualElement()
            {
                style = {
                    width = size,
                    height= size,
                    backgroundColor = new Color(.8f,.8f,.8f),
                }
            };
            circle0.transform.scale = scale; scale.x -= .2f; scale.y -= .2f;
            circle1.transform.scale = scale; scale.x -= .05f; scale.y -= .05f;
            circle2.transform.scale = scale; scale.x -= .25f; scale.y -= .25f;
            circle3.transform.scale = scale;
            circle0.style.SetRS_Style(RSRadius.Percent(15));
            circle1.style.SetRS_Style(radius);
            circle2.style.SetRS_Style(radius);
            circle0.Add(circle1);
            circle1.Add(circle2);
            circle2.Add(circle3);
            return circle0;
        }
        public override void LayoutInline()
        {
            if (m_isDrawRefence) return;
            FoldoutState = false;
            m_isDrawRefence = true;
            base.LayoutInline();
            contentContainer.style.display = DisplayStyle.None;
            if (objRefPicker != null)
                objRefPicker.style.display = DisplayStyle.Flex;
        }
        public override void LayoutExpand()
        {
            if (!m_isDrawRefence) return;
            FoldoutState = false;
            m_isDrawRefence = false;
            base.LayoutExpand();
            contentContainer.style.display = DisplayStyle.Flex;
            if (objRefPicker != null)
                objRefPicker.style.display = DisplayStyle.None;
            if (DynamicLayout)
            {
                var originIndent = RSTheme.indentLevel;
                RSTheme.indentLevel = NestLevel + 1;
                CreateGUI();
                RSTheme.indentLevel = originIndent;
            }
            RepaintDrawer();
        }
        public override void RepaintDrawer()
        {
            if (m_isDrawRefence)
                (objRefPicker as INotifyValueChanged<string>).SetValueWithoutNotify($"{(value == null ? "None" : value.name)} ({TypeReader.GetName(objectType)})");
            else
                base.RepaintDrawer();
        }
    }
}
