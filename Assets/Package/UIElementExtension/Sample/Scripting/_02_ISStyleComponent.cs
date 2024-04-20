using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK.Sample
{
    /* #1 Intro */
    /*------------------------------------------------------------------------------------------------------------|
    | RSStyleComponent is a base class for RSStyle Group, here shows the feature that can be use in any SubClass  |
    |------------------------------------------------------------------------------------------------------------*/
    public class _02_RSStyleComponent
    {
        // All class in RSStyle Group
        // RSStyle include all of other Component below
        public RSStyle Style = new(); 

        public RSDisplay    RSDisplay    = new();
        public RSPosition   RSPosition   = new();
        public RSFlex       RSFlex       = new();
        public RSAlign      RSAlign      = new();
        public RSSize       RSSize       = new();
        public RSMargin     RSMargin     = new();
        public RSPadding    RSPadding    = new();
        public RSText       RSText       = new();
        public RSBackground RSBackground = new();
        public RSBorder     RSBorder     = new();
        public RSRadius     RSRadius     = new();
        public RSTransform  RSTransform  = new();

        void BasicProperties()
        {
            RSStyleComponent component = null; // It's a Abstract, we can not construct it.

            // How many properties in this Style
            _ = component.PropertyCount;

            // What kind of style is it, see RSStyle for detail
            _ = component.StyleFlag;

            // What properties has been set, it's a int 32 Flag
            _ = component.SetUnsetFlag;
        }

        /* #2 Set Unset Flag */
        /*------------------------------------------------------------------|
        | In UITK UI Builder, we have ability set or unset a property.      |
        | In RSStyle, we use a Int32 flag to represent it.                  |
        | If a value is unset, all of releate operation will ignore it.     |
        | Here we use RSAlign as example, because it only has 3 properties. |
        |------------------------------------------------------------------*/
        void StyleProperties()
        {
            RSAlign align = new RSAlign();

            // the properties in RSAlign
            _ = align.alignSelf;
            _ = align.alignItems;
            _ = align.justifyContent;

            // check a property(here is AlignSelf) is Set or Unset
            _ = align.GetFlag(RSAlign.F_AlignSelf);
            // check is All properties is set ?
            _ = align.GetFlag(RSAlign.F_Any);

            // change Set/Unset for properties
            align.SetFlag(RSAlign.F_AlignSelf, true);
            align.SetFlag(RSAlign.F_Any, false);
            // or you can change flag directed
            align.SetUnsetFlag = RSAlign.F_AlignSelf; // only "Set" AlignSelf, other "Unset"
            align.SetUnsetFlag = RSAlign.F_Any & ~RSAlign.F_AlignSelf; // only "Unset" AlignSelf, other "Set"

            // other shortcut
            align.SetAll();   // all properties turn into "Set"
            align.UnsetAll(); // all properties turn into "Unset"

            // you can also make property to default value.
            // This function don't change Set/Unset flag on properties
            align.SetValueToDefault(RSAlign.F_Any); // make all properties to default

            // when you define(invoke setter) a property,
            // it will auto turn into Set state, for example:
            /* False */_ = align.GetFlag(RSAlign.F_AlignSelf);
                           align.alignSelf = Align.FlexEnd;
            /* True  */_ = align.GetFlag(RSAlign.F_AlignSelf);
        }

        /* #3 Operate Style on VisualElement */
        /*------------------------------------------------------------------|
        | This is Main part of RS Style, we have those main operate below:  |
        |   - Copy Style from other RSStyle                                 |
        |   - Copy Style from a VisualElement                               |
        |   - Copy Style between 2 RSStyle by Lerp                          |
        |   - Apply Style on a VisualElement                                |
        |   - Apply Style on a VisualElement with Lerp [0,1f]               |
        |------------------------------------------------------------------*/
        void StyleOperate()
        {
            VisualElement visualElement = new();
            RSAlign align1 = new();
            RSAlign align2 = new();

            // init RSStyle
            align1.LoadFrom(visualElement); // Copy value from VisualElement, this will make all properties into "Set"
            align2.LoadFrom(align1);        // Copy value from Other
            align2.LoadFromIfUnset(align1); // Copy value from Other if (self is unset && other is set)

            // static init shortcut
            align1 = RSAlign.CreateFrom(visualElement);
            align2 = RSAlign.CreateFrom(align1);

            // Load value between 1 & 2
            // Only load "Set" properties on both side (so the new set/unset = (flag1 & flag2))
            RSAlign align3 = new();
            align3.LoadFromLerp(align1, align2, 0.5f);
            // Lerp version of LoadFromIfUnset()
            align3.LoadFromLerpIfUnset(align1, align2, 0.5f); 


            // Apply style on Element, calling it from Element.style will make it easier to iterate muiltiple elements
            align1.ApplyOn(visualElement);           // method 1 
            visualElement.style.SetRS_Style(align1); // method 2

            // This is a shorcut to apply Lerped value on a element
            // equal to :
            //      RSAlign temp = new();
            //      temp.LoadFromLerp(align1, align2, 0.5f);
            //      temp.ApplyOn(visualElement);
            align1.ApplyTransitionOn(visualElement.style, align2, 0.5f);


            // A vary useful example :
            // if we already have a well styled element, than we can copy its style and apply on new one
            VisualElement wellStyledElement = null;

            VisualElement newElement = new();
            RSStyle style = RSStyle.CreateFrom(wellStyledElement);
            newElement.style.SetRS_Style(style);
        }
    }
}