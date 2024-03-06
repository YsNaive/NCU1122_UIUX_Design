using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace NaiveAPI.RuntimeWindowUtils
{
    [CustomRuntimeDrawer(typeof(GameObject), Priority = -100)]
    public class GameObjectDrawer : RuntimeDrawer<GameObject>
    {
        public override bool DynamicLayout => true;
        public GameObjectDrawer() {
            style.SetIS_Style(new ISBorder(DocStyle.Current.SubBackgroundColor, 1.5f));
            iconElement.style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            iconElement.style.width = StyleKeyword.Auto;
            iconElement.Add(new DSTextElement("GameObject")
            {
                style =
                {
                    color = DocStyle.Current.FrontgroundColor,
                    unityTextAlign = TextAnchor.UpperCenter,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    paddingLeft = 5,
                    paddingRight = 5,
                }
            });
            LayoutExpand(); 
        }
        protected override void OnCreateGUI()
        {
            Clear();
            if (value == null) return;
            label = value.name;
            foreach (var component in value.GetComponents<Component>())
            {
                RuntimeDrawer drawer;
                Add(drawer = Create(component, TypeReader.GetName(component.GetType())));
                drawer.style.borderTopColor = DocStyle.Current.FrontgroundColor;
                drawer.style.borderTopWidth = 1.5f;
                drawer.labelElement.style.letterSpacing = DocStyle.Current.MainTextSize/4;
                var asUnityObjectDrawer = drawer as IUnityObjectDrawer;
                if (asUnityObjectDrawer != null)
                {
                    asUnityObjectDrawer.LayoutExpand();
                    Behaviour asBehaviour = component as Behaviour;
                    Renderer asRenderer = component as Renderer;
                    if (asBehaviour != null)
                    {
                        DSToggle toggle = new();
                        toggle.style.position = Position.Absolute;
                        toggle.style.left = -DocStyle.Current.LineHeight.Value;
                        toggle.SetValueWithoutNotify(asBehaviour.enabled);
                        toggle.RegisterValueChangedCallback(evt => asBehaviour.enabled = evt.newValue);
                        drawer.titleElement.Add(toggle);
                    }
                    else if (asRenderer != null)
                    {
                        DSToggle toggle = new();
                        toggle.style.position = Position.Absolute;
                        toggle.style.left = -DocStyle.Current.LineHeight.Value;
                        toggle.SetValueWithoutNotify(asRenderer.enabled);
                        toggle.RegisterValueChangedCallback(evt => asRenderer.enabled = evt.newValue);
                        drawer.titleElement.Add(toggle);
                    }
                }
                else
                    drawer.SetValueWithoutNotify(component);
                //if (component is Transform) continue;
                //var hor = drawer.Q<DSHorizontal>();
                //var btn = new DSTextElement("remove")
                //{
                //    style =
                //    {
                //        color = DocStyle.Current.DangerTextColor,
                //        backgroundColor = DocStyle.Current.DangerColor,
                //        marginRight = 0,
                //        marginLeft = StyleKeyword.Auto,
                //        paddingLeft = 5,
                //        paddingRight = 5,
                //        borderLeftColor = DocStyle.Current.FrontgroundColor,
                //        borderLeftWidth = 1.5f,
                //    }
                //};
                //btn.RegisterCallback<PointerDownEvent>(evt =>
                //{
                //    Object.Destroy(component);
                //    drawer.parent?.Remove(drawer);
                //});
                //hor.Add(btn);
            }
        }
        public override void UpdateField()
        {
            foreach (var drawer in Children())
            {
                if((drawer as IFoldoutDrawer)?.FoldoutState ?? true)
                    (drawer as RuntimeDrawer)?.UpdateField();
            }
        }
    }
}