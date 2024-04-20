using UnityEngine;

namespace NaiveAPI.UITK.Sample
{
    /* RuntimeDrawer is implement in UIElement */
    /* #1 Intro */
    /*--------------------------------------------------------------|
    | Runtime Drawer is use as field like IntField, FloatField etc. |
    | Here show you how to use this system.                         |
    |--------------------------------------------------------------*/
    public class _06_RuntimeDrawer
    {
        class SampleData
        {
            [Header("Player Settings")]
            public string name;
            [Range(0, 100)]
            public int power;
            public int hp;
        }

        void BasicProperties()
        {
            var drawer = new IntegerDrawer();
            // lable as string or RSLocalizeText
            _ = drawer.label;
            _ = drawer.localizeLabel;

            // auto constructed element
            // check more layout info on pdf docs
            _ = drawer.iconElement;
            _ = drawer.labelElement;
            _ = drawer.contentContainer;

            // if you need, you can add tooltips
            _ = drawer.tooltipElement;
            _ = drawer.tooltipElement.PopupDelay;

            // layout settings
            _ = drawer.labelWidth;
            _ = drawer.indentWidth;

            // value below can't be change and auto assign when constructed
            // those value measure base on RSTheme.indentLevel
            _ = drawer.NestLevel;
            _ = drawer.MeasuredLabelWidth;
            _ = drawer.MeasuredIndentWidth;
        }

        public void CreateDrawer()
        {
            // create drawer by just construct it
            _ = new IntegerDrawer();

            // create drawer from any object
            // it will auto find its drawer type
            _ = RuntimeDrawer.Create(10);

            // create drawer from any object and Attribute
            // for example: int + RangeAttribute = IntRangeDrawer
            // Attribute will only effect if there has fit Drawer
            // You can check customize Part to know more about it
            _ = RuntimeDrawer.Create(10, "label", new RangeAttribute(0, 100));
            _ = RuntimeDrawer.Create(10, "label", new RangeAttribute(0, 100), new HeaderAttribute("Header"));

            // create drawer from member
            // it will auto find its Attributies
            var data = new SampleData();
            _ = RuntimeDrawer.CreateAndBind(data, nameof(data.name));
            _ = RuntimeDrawer.CreateAndBind(data, nameof(data.hp));
        }

        /*--------------------------------------------------------|
        | use event to get callback when value change             |
        | OnValueChanged => Invoke when value change (reference)  |
        | OnMemberValueChanged => Invoke when member value change |
        |--------------------------------------------------------*/
        public void Usage()
        {
            var data = new SampleData();

            // normal way to create drawer for data.hp
            var hpDrawer = new IntegerDrawer { label = "HP" };
            hpDrawer.value = data.hp;
            hpDrawer.OnValueChanged += () =>
            {
                data.hp = hpDrawer.value;
                // other code invoke when value change
                // ...
            };

            // fast way to create drawer for data.hp
            // if you don't need to do other thing when value change
            var hpDrawer2 = RuntimeDrawer.CreateAndBind(data, nameof(data.hp));
            hpDrawer2.Bind(data, nameof(data.hp));

            // you can even just create a drawer for data without any settings
            var dataDrawer = RuntimeDrawer.Create(data, "Player Data");
            dataDrawer.OnMemberValueChanged += (changedDrawer) =>
            {
                // when it member drawer value changed
                // it could be data.name / data.power / data.hp
                // pass in param is the changed drawer
                if(changedDrawer.label == "name")
                {
                    // name changed
                    Debug.Log($"New name = {data.hp}");
                }
            };
        }

        void Misc()
        {
            var drawer = new IntegerDrawer();
            // change current layout
            drawer.LayoutInline();
            drawer.LayoutExpand();

            // normal set
            drawer.value = 10;
            // set without invoke event
            drawer.SetValueWithoutNotify(10);
            // set without repaint drawer
            drawer.SetValueWithoutRepaint(10);
        }
    }
}
