using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    [CustomRuntimeDrawer(typeof(GameObject), Priority = 0)]
    public class GameObjectDrawer : UnityObjectDrawer<GameObject>
    {
        public override bool DynamicLayout => true;
        public GameObjectDrawer()
        {
            SetIcon(DefaultDrawer.GetDefaultIcon(typeof(GameObject)));
            style.SetRS_Style(new RSBorder { anyColor = RSTheme.Current.BackgroundColor2, anyWidth = 1 });
        }
        protected override void CreateGUI()
        {
            Clear();
            label = "-";
            if (value == null) return;
            label = value.name;
            foreach (var component in value.GetComponents<Component>())
            {
                var localComponent = component;
                var drawer = AddDrawer(TypeReader.GetName(component.GetType()),
                    () => localComponent,
                    (v) => { });
                drawer.SetValue(component);
                drawer.LayoutExpand();
                drawer.style.marginTop = 2;
                drawer.labelElement.style.letterSpacing = RSTheme.Current.TextSize/4f;
                drawer.contentContainer.style.paddingBottom = RSTheme.Current.LineHeight / 3f;
            }
        }
        public override void RepaintDrawer()
        {
            foreach (var drawer in Children())
            {
                if((drawer as IFoldoutDrawer)?.FoldoutState ?? true)
                    (drawer as RuntimeDrawer)?.RepaintDrawer();
            }
        }
    }
}