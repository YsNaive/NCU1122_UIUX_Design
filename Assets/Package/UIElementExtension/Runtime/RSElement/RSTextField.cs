using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public class RSTextField : TextField
    {
        public new class UxmlFactory : UxmlFactory<RSTextField, TextField.UxmlTraits> { }
        public VisualElement InputFieldElement => inputFieldElement;
        VisualElement inputFieldElement;
        public RSTextField()
        {
            style.minHeight = RSTheme.Current.LineHeight;
            style.ClearMarginPadding();
            style.SetRS_Style(RSTheme.Current.MainText);
            inputFieldElement = this[0];
            inputFieldElement.style.ClearMarginPadding();
            inputFieldElement.style.SetRS_Style(RSTheme.Current.FieldStyle);
            inputFieldElement.style.backgroundColor = RSTheme.Current.BackgroundColor3;
            inputFieldElement.style.color = RSTheme.Current.FrontgroundColor2;
        }
        public new string label
        {
            get => base.label;
            set
            {
                base.label = value;
                labelElement.style.SetRS_Style(RSTheme.Current.MainText);
                labelElement.style.width = RSTheme.Current.LabelWidth;
                labelElement.style.minWidth = RSTheme.Current.LabelWidth;
            }
        }
        public RSTextField(string label) : this()
        {
            this.label = label;
        }
        public RSTextField(string label, EventCallback<ChangeEvent<string>> changeCallback) : this(label)
        {
            this.RegisterValueChangedCallback(changeCallback);
        }
    }
}
