using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK.Sample
{
    /* #1 Intro */
    /*--------------------------------------|
    | Here is steps to use RuntimeWIndow    |
    |  1. Setup Unity UID in Editor         |
    |  2. Setup RuntimeWindow ScreenElement |
    |  3. Create Your Window                |
    |  4. Use in Runtime !                  |
    |--------------------------------------*/
    public class _09_RuntimeWindow
    {
        void Setup()
        {
            // You need to define ScreenElement before use any method in RuntimeWindow
            VisualElement yourElement = Object.FindObjectOfType<UIDocument>().rootVisualElement;
            RuntimeWindow.ScreenElement = yourElement;
        }

        /* #2 Define Window */
        /*-----------------------------------|
        | this is how you Define your Window |
        | vary easy and simple               |
        |-----------------------------------*/
        class SampleWindow : RuntimeWindow
        {
            // you can extand from RSRuntimeWindow that styled by RSTheme
            public SampleWindow()
            {
                // first, define the layout: Percent or Pixel
                InitLayoutAsPercent(new Rect(0, 0, 0.5f, 0.5f));
                InitLayoutAsPixel(new Rect(0, 0, 600, 800));

                // than modify default settings
                Dragable = false;
                Resizable = true;
                MinSize = new Vector2(0.1f, 0.1f);
                // ... etc.
            }
        }
        void BasicUsage()
        {
            // get your window, it will also auto popup in ScreenElement
            // get window will try to find first instance of type "T" window
            // if not found, create new one
            var window = RuntimeWindow.GetWindow<RSRuntimeWindow>();
            // use CreateWindow will always create a new one
            window = RuntimeWindow.CreateWindow<RSRuntimeWindow>();

            // now you can do anything with it as a VisualElement
            window.Add(new RSTextElement("TEXT"));
            window.Add(new RSButton("BUTTON"));

            // close window but not destory (keep instance)
            window.Close();
            // destory and close window
            window.Destory();


            /*--------------------------------------------------------------|
            | above is how you use RuntimeWindow in normal                  |
            | but you still can use it as a VisualElement                   |
            | in that case, you don't need to care about Destory() function |
            |--------------------------------------------------------------*/
            VisualElement root = null;
            window = new RSRuntimeWindow();
            root.Add(window);
        }

        /* #3 Properties */
        /*--------------------------------------------------|
        | There are many build-in feature for RuntimeWindow |
        | It's better to know them before start using       |
        |--------------------------------------------------*/
        void BasicProperties()
        {
            var window = RuntimeWindow.GetWindow<RSRuntimeWindow>();
            // Here are some build in properties to modify
            // Define Coord for this window is Pixel or Percent[0,1f]
            _ = window.LayoutUnit;

            // if unit is percent, all settings below using as [0,1f]
            _ = window.MinSize;
            _ = window.MaxSize;
            // last size calculated by window,
            // this allow you to make its size back after you modify it youself
            _ = window.LastMeasureSize;
            // set Size & Position, but NOT effect immediately
            _ = window.NextSize;
            _ = window.NextPosition;
            window.SetSize(new Vector2());
            window.SetPosition(new Vector2());
            window.SetLayout(new Rect());

            // limit it's layout in parentElement
            _ = window.LimitInParent;
            // limit it's size in [MinSize, MaxSize]
            _ = window.LimitSize;
            // use Anyway version will ignore LimitInParent or LimitSize
            // and calculated result will not assign to LastMeasureSize
            window.SetSizeAnyway(new Vector2());
            window.SetPositionAnyway(new Vector2());
            window.SetLayoutAnyway(new Rect());

            // use those method can help you modify layout, but it only work when WindowParent != null
            // use GetCoordFromPercent/Pixel(), you don't need to care unit of this window
            Rect layout = window.GetCoordFromPercent(new Rect(0, 0, 1, 1)); // full screen
            window.SetLayout(layout);

            // title name
            _ = window.TabName;
            // Open/Close tabElement
            _ = window.EnableTab;
            // Is this Window can Drag by holding tabElement
            _ = window.Dragable;
            // Is this Window can Resize by holding border
            _ = window.Resizable;
            // Snap position to this Anchor (MiddleCenter = disable)
            _ = window.SnapBorder;
            // If true, window will try to fit screen when user drag it close to edge like web browser.
            _ = window.SnapLayout;

            // If true, you can open ContextMenu by mouse right click.
            _ = window.EnableContextMenu;
                window.ContextMenu.Add("menu path", () => { /* Invoke when select */ });
        }
    }
}
