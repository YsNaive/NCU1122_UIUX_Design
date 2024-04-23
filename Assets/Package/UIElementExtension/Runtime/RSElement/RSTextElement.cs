using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public class RSTextElement : TextElement
    {
        //public new class UxmlFactory : UxmlFactory<RSTextElement, UxmlTraits> { }
        public RSTextElement()
        {
            RSTheme.Current.ApplyTextStyle(this);
            style.whiteSpace = WhiteSpace.Normal;
        }
        public RSTextElement(string text) : this()
        {
            this.text = text;
        }
    }
}
