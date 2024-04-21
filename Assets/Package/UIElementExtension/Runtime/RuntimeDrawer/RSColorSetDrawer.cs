using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    [CustomRuntimeDrawer(typeof(RSColorSet))]
    public class RSColorSetDrawer : FoldoutDrawer<RSColorSet>
    {
        public readonly string[] ValueNames = new string[]
        {
            "Text Color",
            "Background",
            "Background 2",
            "Background 3",
            "Frontground",
            "Frontground 2",
            "Frontground 3",
        };
        public ColorDrawer[] ValueDrawers = new ColorDrawer[7];
        public VisualElement[] ColorPreviewElement = new VisualElement[7];
        public VisualElement ColorPreviewContainer;
        PageView<bool> pageView;
        public override void RepaintDrawer()
        {
            for (int i = 0; i < 7; i++)
            {
                ValueDrawers[i].SetValueWithoutNotify(value[i]);
                ColorPreviewElement[i].style.backgroundColor = value[i];
            }
        }

        protected override void CreateGUI()
        {
            pageView = new PageView<bool>(false);
            ColorPreviewContainer = new VisualElement()
            {
                style =
                {
                    position = Position.Absolute,
                    left =  RSTheme.Current.IndentedLabelWidth,
                    flexDirection = FlexDirection.Row,
                }
            };
            labelElement.Add(ColorPreviewContainer);
            var size = RSTheme.Current.LineHeight - RSTheme.Current.VisualMargin;
            var padding = RSTheme.Current.VisualMargin / 2f;
            for (int i = 0; i < 7; i++)
            {
                ColorDrawer colorDrawer = new ColorDrawer() { label = ValueNames[i] };
                ValueDrawers[i] = colorDrawer;
                var localIndex = i;
                colorDrawer.OnValueChanged += () =>
                {
                    value[localIndex] = colorDrawer.value;
                    ColorPreviewElement[localIndex].style.backgroundColor = colorDrawer.value;
                    InvokeMemberValueChange(colorDrawer);
                };
                pageView.Add(colorDrawer);

                VisualElement previewElement = new VisualElement();
                ColorPreviewElement[i] = previewElement;
                previewElement.style.marginTop = padding;
                previewElement.style.marginLeft = padding;
                previewElement.style.height = size;
                previewElement.style.width = size;
                ColorPreviewContainer.Add(previewElement);
            }
            Add(pageView);


            pageView.OpenOrCreatePage(true);
            Color32[] copyBuffer = new Color32[7];
            RSButton generateButton = new RSButton("Generate", RSTheme.Current.HintColorSet);
            RSButton applyButton = new RSButton("Apply", RSTheme.Current.SuccessColorSet);
            RSButton cancelButton = new RSButton("Cancel", RSTheme.Current.DangerColorSet);
            FloatRangeDrawer splitDrawer = new FloatRangeDrawer() { label = "Split" };
            ColorDrawer backgroundColorDrawer = new ColorDrawer() { label = "Background" };
            ColorDrawer frontgroundColorDrawer = new ColorDrawer() { label = "Frontground" };
            pageView.Add(splitDrawer);
            pageView.Add(backgroundColorDrawer);
            pageView.Add(frontgroundColorDrawer);
            splitDrawer.slider.lowValue = 5f;
            splitDrawer.slider.highValue = 15f;
            applyButton.style.display = DisplayStyle.None;
            cancelButton.style.display = DisplayStyle.None;
            generateButton.style.marginLeft = RSTheme.Current.VisualMargin;
            generateButton.clicked += () =>
            {
                FoldoutState = true;
                generateButton.style.display = DisplayStyle.None;
                applyButton.style.display = DisplayStyle.Flex;
                cancelButton.style.display = DisplayStyle.Flex;
                backgroundColorDrawer.SetValueWithoutNotify(value[1]);
                frontgroundColorDrawer.SetValueWithoutNotify(value[6]);
                splitDrawer.SetValueWithoutNotify(9f);
                pageView.OpenPage(true);
                for (int i = 0; i < 7; i++)
                {
                    copyBuffer[i] = value[i];
                }
            };
            applyButton.style.marginLeft = RSTheme.Current.VisualMargin;
            applyButton.clicked += () =>
            {
                generateButton.style.display = DisplayStyle.Flex;
                applyButton.style.display = DisplayStyle.None;
                cancelButton.style.display = DisplayStyle.None;
                pageView.OpenPage(false);
            };
            cancelButton.clicked += () =>
            {
                generateButton.style.display = DisplayStyle.Flex;
                applyButton.style.display = DisplayStyle.None;
                cancelButton.style.display = DisplayStyle.None;
                pageView.OpenPage(false);
                for (int i = 0; i < 7; i++)
                    this.value[i] = copyBuffer[i];
                InvokeMemberValueChange(this);
                RepaintDrawer();
            };
            ColorPreviewContainer.Add(generateButton);
            ColorPreviewContainer.Add(applyButton);
            ColorPreviewContainer.Add(cancelButton);

            Action gengerateEvent = () =>
            {
                value.Generate(backgroundColorDrawer.value, frontgroundColorDrawer.value, splitDrawer.value);
                InvokeMemberValueChange(this);
                RepaintDrawer();
            };
            backgroundColorDrawer.OnValueChanged += gengerateEvent;
            frontgroundColorDrawer.OnValueChanged += gengerateEvent;
            splitDrawer.OnValueChanged += gengerateEvent;

            pageView.OpenPage(false);
        }

    }
}
