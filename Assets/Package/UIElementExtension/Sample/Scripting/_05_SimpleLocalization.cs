
using UnityEngine.UIElements;

namespace NaiveAPI.UITK.Sample
{
    /* #1 Intro */
    /*----------------------------------------------------------------------------|
    | RSLocalization can help to localize text or sprite, here you need to preset |
    | some value in Editor, please check pdf docs.                                |
    |----------------------------------------------------------------------------*/
    public class _05_SimpleLocalization
    {
        /*-----------------------------------------------------|
        | useful static function and basic settings in runtime |
        |-----------------------------------------------------*/
        void _RSLocalization()
        {
            // change language
            // if can't found value from CurrentLanguage 
            // will try get from DefaultLanguage
            RSLocalization.CurrentLanguage = "zh-CHT";
            RSLocalization.DefaultLanguage = "en";

            // get text, font or sprite from key
            // return null if key not exist
            _ = RSLocalization.GetSprite("key");
            // return {key} if key not exist
            _ = RSLocalization.GetText("key");
            // return ({key}, null) if key not exist
            (var text, var font) = RSLocalization.GetTextAndFont("key");

            // get loaded LanguageKey
            // en, zh-CHT etc.
            _ = RSLocalization.LanguageKeys;

            // if you really need, you can add key-value yourself in runtime
            // but it's better to add it in Editor
            _ = RSLocalization.GetTextTable("language key");
            _ = RSLocalization.GetSpriteTable("language key");
        }

        /*-------------------------------------|
        | a data type to operate LocalizeText  |
        |-------------------------------------*/
        void _RSLocalizeText()
        {
            // You can create RSLocalizeText by key or by value
            _ = RSLocalizeText.FromKey("key");
            _ = RSLocalizeText.FromValue("value");

            var localizeText = new RSLocalizeText();
            // this bool tell you is this LocalizeText just a string value
            // or will try to find localizeValue in table
            _ = localizeText.IsBinding;

            // setting after construct
            // when you invoke those setter, they will change its Binding
            // set key   will also set IsBinding = true
            // set value will also set IsBinding = false
            localizeText.key = "key";
            localizeText.value = "value";

            // get text
            // if IsBinding is T will try to get its text in current language
            // if IsBinding is F just return value
            _ = localizeText.value;
            _ = localizeText.FontAsset;
        }

        /*-----------------------------------------|
        | a VisualElement to display LocalizeText  |
        |-----------------------------------------*/
        void _RSLocalizeTextElement()
        {
            // if construct with a string, it will just like a TextElement
            _ = new RSLocalizeTextElement("value");
            _ = new RSLocalizeTextElement(RSLocalizeText.FromKey("key"));

            // dont use text to modify value, use value or key
            var element = new RSLocalizeTextElement();
            element.value = "Title";
            element.key = "key.title";

            // if CurrentLangeage is changed, you can use this method to reload.
            element.ReloadText();

            // if you want to reload a VisualTree
            var visualTree = new VisualElement();
            foreach (var item in visualTree.HierarchyRecursive<RSLocalizeTextElement>())
                item.ReloadText();
        }
    }
}
