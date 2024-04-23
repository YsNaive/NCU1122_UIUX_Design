using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    [CustomRuntimeDrawer(typeof(bool), Priority = 10)]
    public class BoolDrawer : RuntimeDrawer<bool>
    {
        public new class UxmlFactory : UxmlFactory<BoolDrawer, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private UxmlStringAttributeDescription m_label = new UxmlStringAttributeDescription
            {
                name = "label",
                defaultValue = "Toggle",
            };
            private UxmlBoolAttributeDescription m_value = new UxmlBoolAttributeDescription
            {
                name = "value",
                defaultValue = false,
            };
            private UxmlBoolAttributeDescription m_frontBox = new UxmlBoolAttributeDescription
            {
                name = "front-box",
                defaultValue = false,
            };
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                BoolDrawer element = ve as BoolDrawer;
                element.label = m_label.GetValueFromBag(bag, cc);
                element.SetValueWithoutNotify(m_value.GetValueFromBag(bag, cc));
                element.frontBox = m_frontBox.GetValueFromBag(bag, cc);
            }
        }

        public bool frontBox
        {
            get => m_frontBox;
            set
            {
                if (m_frontBox != value)
                {
                    if (value)
                    {
                        labelElement.BringToFront();
                        labelElement.style.paddingLeft = RSTheme.Current.LineHeight / 2f;
                    }
                    else
                    {
                        contentContainer.BringToFront();
                        labelElement.style.paddingLeft = 0;
                    }
                    m_frontBox = value;
                }
            }
        }

        private bool m_frontBox = false;
        Toggle toggle;
        protected override void CreateGUI()
        {
            contentContainer.style.flexGrow = 0;
            toggle = new Toggle();
            toggle.style.ClearMarginPadding();
            toggle.style.minHeight = RSTheme.Current.LineHeight;
            toggle.style.color = RSTheme.Current.NormalColorSet.TextColor;
            toggle.style.fontSize = RSTheme.Current.TextSize;
            var checkMark = toggle.Q("unity-checkmark");
            checkMark.style.SetRS_Style(new RSBorder(RSTheme.Current.FrontgroundColor, 1.5f));
            checkMark.style.backgroundColor = RSTheme.Current.BackgroundColor2;
            checkMark.style.unityBackgroundImageTintColor = RSTheme.Current.FrontgroundColor;
            checkMark.style.width = RSTheme.Current.LineHeight;
            checkMark.style.height = RSTheme.Current.LineHeight;
            checkMark.style.scale = new Scale(new Vector3(.7f, .7f, 1.0f));
            toggle.RegisterValueChangedCallback(evt =>
            {
                SetValueWithoutRepaint(evt.newValue);
                evt.StopPropagation();
            });
            Add(toggle);
        }
        public override void RepaintDrawer()
        {
            toggle.SetValueWithoutNotify(value);
        }
    }
}