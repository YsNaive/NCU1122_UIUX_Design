using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK.Sample
{
    /* RuntimeDrawer is implement in UIElement */
    /* #1 Intro */
    /*------------------------------------------------------------------|
    | You Should Read All Releated Info in PDF Docs Before this Part    |
    | You Should Read All Releated Info in PDF Docs Before this Part    |
    | You Should Read All Releated Info in PDF Docs Before this Part    |
    |                                                                   |
    | here give some example to implement a CustomTypeDrawer            |
    |                                                                   |
    |---------------------------  Importent  ---------------------------|
    | if your value type is a UnityObject, it's better to extand from   |
    | UnityObjectDrawer<T>, this template extand from StandardDrawer<T> |
    | check StandardImplement if you need.                              |
    |------------------------------------------------------------------*/
    public class _07_CustomRuntimeDrawer
    {
        /*---------------------|
        | Static Layout Sample |
        |---------------------*/
        class SampleData
        {
            public int intValue;
            public float floatValue;
            public Vector2 vector2Value;
        }
        // Define Attribute is not Require
        // But if you defind it, youc can register this Drawer in Runtime.Create()
        // [CustomRuntimeDrawer(typeof(SampleData), Priority = 0)]
        class NormalImplement : RuntimeDrawer<SampleData>
        {
            IntegerDrawer integerDrawer;
            FloatDrawer   floatDrawer;
            Vector2Drawer vector2Drawer;
            public override void RepaintDrawer()
            {
                integerDrawer.value = value.intValue;
                floatDrawer.value   = value.floatValue;
                vector2Drawer.value = value.vector2Value;
            }

            protected override void CreateGUI()
            {
                integerDrawer = new IntegerDrawer() { label = "int"};
                floatDrawer   = new FloatDrawer()   { label = "float"};
                vector2Drawer = new Vector2Drawer() { label = "vector2"};

                integerDrawer.OnValueChanged += () => value.intValue = integerDrawer.value;
                floatDrawer.OnValueChanged   += () => value.floatValue = floatDrawer.value;
                vector2Drawer.OnValueChanged += () => value.vector2Value = vector2Drawer.value;

                Add(integerDrawer);
                Add(floatDrawer);
                Add(vector2Drawer);
            }
        }
        class StandardImplement : StandardDrawer<SampleData>
        {
            protected override void CreateGUI()
            {
                AddDrawer("int"    , () => value.intValue    , (v) => value.intValue = v);
                AddDrawer("float"  , () => value.floatValue  , (v) => value.floatValue = v);
                AddDrawer("vector2", () => value.vector2Value, (v) => value.vector2Value = v);
            }
        }



        /*----------------------|
        | Dynamic Layout Sample |
        |----------------------*/
        class BaseData
        {
            public int i = 0;
        }
        class ChildData : BaseData
        {
            public int j = 0;
        }
        /*--------------------------------------------|
        | A Dynamic Layout sample,                    |
        | assume that your value is Polymorphism      |
        | but actually you can just use DefaultDrawer |
        |--------------------------------------------*/
        // You need to set DrawDerivedType = true if you want to register it
        // or it will only match BaseData without ChildData
        // [CustomRuntimeDrawer(typeof(BaseData), DrawDerivedType = true, Priority = 0)]
        class NormalDynamic : RuntimeDrawer<BaseData>
        {
            public override bool DynamicLayout => true;
            IntegerDrawer iDrawer, jDrawer;
            public override void RepaintDrawer()
            {
                iDrawer.value = value.i;
                if (jDrawer != null)
                    jDrawer.value = (value as ChildData).j;
            }

            protected override void CreateGUI()
            {
                Clear();
                iDrawer = new IntegerDrawer();
                iDrawer.OnValueChanged += () => value.i = iDrawer.value;
                Add(iDrawer);

                if (value is ChildData)
                {
                    jDrawer = new IntegerDrawer();
                    jDrawer.OnValueChanged += () => (value as ChildData).j = jDrawer.value;
                    Add(jDrawer);
                }
                else
                {
                    jDrawer = null;
                }
            }
        }
        class StandardDynamic : StandardDrawer<BaseData>
        {
            public override bool DynamicLayout => true; 
            protected override void CreateGUI()
            {
                ClearDrawer();
                AddDrawer("i", () => value.i, (v) => value.i = v);
                if(value is ChildData)
                    AddDrawer("j", () => (value as ChildData).j, (v) => (value as ChildData).j = v);
            }
        }

        void Debug()
        {
            VisualElement visualTree = new();
            // you can use this function to add debug tooltip on VisualTree
            RuntimeDrawer.Debug_AddPropertiesTooltips(visualTree);
        }
    }
}
