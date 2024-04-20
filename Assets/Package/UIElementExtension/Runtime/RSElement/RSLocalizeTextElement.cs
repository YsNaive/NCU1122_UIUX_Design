using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public class RSLocalizeTextElement : RSTextElement
    {
        public RSLocalizeText localizeText;
        public string value
        {
            get => localizeText.value;
            set
            {
                localizeText.value = value;
                ReloadText();
            }
        }
        public string key
        {
            get => localizeText.key;
            set
            {
                localizeText.key = value;
                ReloadText();
            }
        }
        public bool IsBinding => localizeText.IsBinding;
        public RSLocalizeTextElement(string value = "")
        {
            localizeText = RSLocalizeText.FromValue(value);
        }
        public RSLocalizeTextElement(RSLocalizeText localizeText)
        {
            this.localizeText = localizeText;
            ReloadText();
        }
        public void ReloadText()
        {
            localizeText.Reload();
            text = localizeText.value;
            if (localizeText.FontAsset != null)
            {
                style.unityFontDefinition = new FontDefinition() { fontAsset = localizeText.FontAsset };
                style.unityFont = localizeText.FontAsset.sourceFontFile;
            }
        }
    }
}
