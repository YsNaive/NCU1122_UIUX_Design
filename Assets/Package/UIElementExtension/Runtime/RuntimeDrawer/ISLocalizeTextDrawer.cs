using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    [CustomRuntimeDrawer(typeof(RSLocalizeText))]
    public class RSLocalizeTextDrawer : RuntimeDrawer<RSLocalizeText>
    {
        static PopupElement popupElement;
        RSButton pickBtn;
        RSTextField textField;
        VisualElement earthIcon;
        
        protected override void CreateGUI()
        {
            if(popupElement == null)
            {
                popupElement = new();
                popupElement.style.minWidth = RSTheme.Current.LabelWidth * 1.5f;
                popupElement.style.minHeight = RSTheme.Current.LabelWidth * 2f;
                popupElement.style.SetRS_Style(new RSBorder { anyColor = RSTheme.Current.FrontgroundColor, anyWidth = 1.5f });
            }
            contentContainer.style.flexDirection = FlexDirection.Row;
            textField = new RSTextField();
            textField.style.flexGrow = 1;
            textField.style.marginRight = 4;
            Add(textField);
            pickBtn = new RSButton("Pick") { style = { minWidth = RSTheme.Current.LabelWidth / 2f } };
            pickBtn.style.marginRight = 4;
            Add(pickBtn);
            earthIcon = new VisualElement()
            {
                style =
                {
                    height = RSTheme.Current.LineHeight,
                    width = RSTheme.Current.LineHeight,
                }
            };
            Add(earthIcon);

            earthIcon.RegisterCallback<PointerDownEvent>(evt =>
            {
                evt.StopImmediatePropagation();
                value.IsBinding = !value.IsBinding;
                RepaintDrawer();
            });
            textField.RegisterValueChangedCallback(evt =>
            {
                evt.StopImmediatePropagation();
                if (value.IsBinding)
                    value.key = evt.newValue;
                else
                    value.value = evt.newValue;
            }); 
            pickBtn.clicked += () =>
            {
                popupElement.Add(RSLocalizationKeyPicker.GetTextKeyPicker(evt => {
                    popupElement.Close();
                    value.key = evt;
                    RepaintDrawer();
                }));
                popupElement.OpenBelow(pickBtn);
            };
        }
        public override void RepaintDrawer()
        {
            earthIcon.style.backgroundImage = Background.FromSprite(value.IsBinding ? RSTheme.Current.Icon.earth : RSTheme.Current.Icon.disableEarth);
            ((INotifyValueChanged<string>)textField).SetValueWithoutNotify(value.IsBinding ? value.key : value.value);
            pickBtn.style.display = value.IsBinding ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
