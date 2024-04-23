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
            RSTheme.Current.ApplyTextStyle(this);
            RSTheme.Current.ApplyTextStyle(labelElement);
            style.ClearMarginPadding();
            inputFieldElement = this[0];
            inputFieldElement.style.ClearMarginPadding();
            RSTheme.Current.ApplyFieldStyle(inputFieldElement);
        }
        public new string label
        {
            get => base.label;
            set
            {
                base.label = value;
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
