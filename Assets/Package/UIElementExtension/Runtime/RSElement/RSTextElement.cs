using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public class RSTextElement : TextElement
    {
        public new class UxmlFactory : UxmlFactory<RSTextElement, UxmlTraits> { }
        public RSTextElement()
        {
            style.whiteSpace = WhiteSpace.Normal;
            style.minHeight = RSTheme.Current.LineHeight;
            style.SetRS_Style(RSTheme.Current.MainText);
            style.color = RSTheme.Current.TextColor;
        }
        public RSTextElement(string text) : this()
        {
            this.text = text;
        }
    }
}
