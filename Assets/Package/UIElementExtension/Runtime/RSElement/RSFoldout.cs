using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public class RSFoldout : Foldout
    {
        public new class UxmlFactory : UxmlFactory<RSFoldout, UxmlTraits> { }
        public new class UxmlTraits : Foldout.UxmlTraits { }
        public Toggle ToggleElement;
        public RSFoldout()
        {
            RSTheme.Current.ApplyTextStyle(this);
            contentContainer.style.paddingLeft = RSTheme.Current.LineHeight / 2f;
            ToggleElement = this.Q<Toggle>();
            ToggleElement.style.ClearMarginPadding();
            ToggleElement.style.backgroundColor = RSTheme.Current.BackgroundColor * 0.65f;
            ToggleElement[0].focusable = false;
            var img = ToggleElement[0][0];
            img.ClearClassList();
            img.style.backgroundImage = Background.FromSprite(RSTheme.Current.Icon.arrow);
            img.style.unityBackgroundImageTintColor = RSTheme.Current.BackgroundColor3;
            img.style.scale = new Scale(new Vector3(.75f, .75f, 1f));
            img.style.width = RSTheme.Current.LineHeight;
            img.style.height = RSTheme.Current.LineHeight;
            img.style.unitySliceBottom = 0;
            img.style.unitySliceLeft = 0;
            img.style.unitySliceRight = 0;
            img.style.unitySliceTop = 0;
            ToggleElement.RegisterValueChangedCallback(e =>
            {
                img.style.rotate = new Rotate(e.newValue ? 90 : 0);
            });
            schedule.Execute(() => { img.style.rotate = new Rotate(value ? 90 : 0); });
            var parent = ToggleElement[0];
        }
        public RSFoldout(string text) : this()
        {
            this.text = text;
            ToggleElement[0].Q<Label>().style.marginLeft = RSTheme.Current.LineHeight / 2f;
        }
    }
}
