using UnityEngine.UIElements;

namespace NaiveAPI.UITK.Sample
{
    public class _02_CopyPasteStyle
    {
        /*----------------------------------------------------------------|
        | You can copy style value from VisualElement or other RSStyle    |
        | this is useful when you need to add similar element in runtime  |
        |----------------------------------------------------------------*/
        public VisualElement existingElement;
        void Copy_and_Paste()
        {
            // Copy Method 1
            RSStyle copiedValue = RSStyle.CreateFrom(existingElement);
            // Copy Method 2
            RSStyle copiedValue2 = new RSStyle();
            copiedValue2.LoadFrom(existingElement);

            // Paste
            VisualElement newElement = new();
            copiedValue.ApplyOn(newElement);
        }
    }
}
