using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace NaiveAPI.RuntimeWindowUtils
{
    public interface IUnityObjectDrawer
    {
        public void LayoutInline();
        public void LayoutExpand();
    }
    public abstract class UnityObjectDrawer<T> : DefaultDrawer<T>, IUnityObjectDrawer
        where T : Object
    {
        public static readonly string[] SkippedProperty = { "tag", "name", "hideFlags", "enabled", "runInEditMode", "useGUILayout" };
        public override bool FieldFilter(FieldInfo info)
        {
            if (info.Name.StartsWith("m_")) return false;
            if (info.Name.StartsWith("s_")) return false;
            return info.IsPublic ? !info.IsDefined(typeof(HideInInspector), false) : info.IsDefined(typeof(SerializeField), false);
        }
        public Type ObjectType
        {
            get => m_ObjectType;
            set => m_ObjectType = value;
        }
        Type m_ObjectType;

        Dictionary<string, Object> choiceDict = new();
        DSTextElement objRefPicker;
        bool m_isDrawRefence = true;
        public UnityObjectDrawer()
        {
            objRefPicker = new DSTextElement();
            objRefPicker.style.maxHeight = DocStyle.Current.LineHeight;
            objRefPicker.style.whiteSpace = WhiteSpace.NoWrap;
            objRefPicker.style.SetIS_Style(DocStyle.Current.InputFieldStyle);
            objRefPicker.style.paddingRight = 0;
            objRefPicker.style.flexGrow = 1;
            (objRefPicker as INotifyValueChanged<string>).SetValueWithoutNotify("None");
            objRefPicker.RegisterCallback<PointerDownEvent>(evt =>
            {
                reloadChoice();
                var popup = DSStringMenu.CreatePopupMenu(choiceDict.Keys.ToList(),
                                  selected => { SetValue(choiceDict[selected]); });
                popup.Open(this);
                popup.transform.position = popup.CoverMask.WorldToLocal(objRefPicker.LocalToWorld(new Vector2(0, objRefPicker.localBound.height)));
                evt.StopPropagation();
            });
            objRefPicker.Add(createObjRefIcon());
            titleElement.Add(objRefPicker);
            LayoutInline();
        }
        void reloadChoice()
        {
            choiceDict.Clear();
            if (m_ObjectType == null) return;
            foreach (var obj in Object.FindObjectsByType(m_ObjectType, FindObjectsSortMode.None))
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
            var radius = ISRadius.Percent(75);
            var size = DocStyle.Current.LineHeight.Value;
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
            circle0.style.SetIS_Style(ISRadius.Percent(15));
            circle1.style.SetIS_Style(radius);
            circle2.style.SetIS_Style(radius);
            circle0.Add(circle1);
            circle1.Add(circle2);
            circle2.Add(circle3);
            return circle0;
        }
        public override void LayoutInline()
        {
            if (m_isDrawRefence) return;
            m_isDrawRefence = true;
            base.LayoutInline();
            contentContainer.style.display = DisplayStyle.None;
            if (objRefPicker != null)
                objRefPicker.style.display = DisplayStyle.Flex;
            titleElement.SetEnabled(true);
        }
        public override void LayoutExpand()
        {
            if (!m_isDrawRefence) return;
            m_isDrawRefence = false;
            base.LayoutExpand();
            if (objRefPicker != null)
                objRefPicker.style.display = DisplayStyle.None;
            if(DynamicLayout)
                OnCreateGUI();
            UpdateField();
        }
        protected override void OnCreateGUI()
        {
            if (m_isDrawRefence)
            {
                m_ObjectType = value?.GetType();
                (objRefPicker as INotifyValueChanged<string>).SetValueWithoutNotify($"{(value == null ? "None" : value.name)} ({TypeReader.GetName(m_ObjectType)})");
            }
            else
            {
                base.OnCreateGUI();
            }
        }
        public override void UpdateField()
        {
            if (m_isDrawRefence)
                (objRefPicker as INotifyValueChanged<string>).SetValueWithoutNotify($"{(value == null ? "None" : value.name)} ({TypeReader.GetName(m_ObjectType)})");
            else
                base.UpdateField();
        }
    }
    [CustomRuntimeDrawer(typeof(Object), Priority = -80, DrawDerivedType = true)]
    public sealed class UnityObjectDrawer : UnityObjectDrawer<Object> { }

    [CustomRuntimeDrawer(typeof(Transform), Priority = -79, DrawDerivedType = true)]
    public sealed class TransformDrawer : UnityObjectDrawer<Transform>
    {
        public override bool DynamicLayout => false;
        protected override void OnCreateGUI()
        {
            AddDrawer("Position", () => value.position,    v => value.position    = v);
            AddDrawer("Rotation", () => value.eulerAngles, v => value.eulerAngles = v);
            AddDrawer("Scale"   , () => value.localScale,  v => value.localScale  = v);
        }
    }

    [CustomRuntimeDrawer(typeof(Camera), Priority = -79, DrawDerivedType = true)]
    public sealed class CameraDrawer : UnityObjectDrawer<Camera>
    {
        public override bool DynamicLayout => false;
        protected override void OnCreateGUI()
        {
            Add(DocVisual.Create(DocDescription.CreateComponent(
                "It is not recommended to modify the camera at runtime.\nIf necessary, you can add a drawer with the target set to the camera and a priority greater than -79.", DocDescription.DescriptionType.Hint)));
        }
    }

}
