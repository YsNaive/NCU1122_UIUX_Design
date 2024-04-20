using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK.Sample
{
    /* #1 Intro */
    /*---------------------------------------------------------------------------------------------------------------|
    | The original VisualElement may use three types to represent length : float / Length / StyleLength.             |
    | We integrate them into a single struct called "RSLength" which allows for implicit conversion from/to other.   |
    | Also has a flag to modify what mode can it be on Inspector settings. Here shows most of operation for RSLength |
    |---------------------------------------------------------------------------------------------------------------*/
    public class _01_RSLength
    {
        void BasicProperties()
        {
            RSLength length = new RSLength();
            length.keyword = StyleKeyword.Auto;
            // while set unit, keyword will auto be Undefined.
            length.unit    = LengthUnit.Pixel; 
            length.value   = 25;

            // will give length = Pixel 25
            Debug.Log(length);
        }
        void ConstructOperate()
        {
            // construct with PixelUnit = 50px
            _ = 50;
            _ = new RSLength(50);
            _ = RSLength.Pixel(50);

            // construct with Percent   = 100%
            _ = new RSLength(LengthUnit.Percent, 100);
            _ = RSLength.Percent(100);
            _ = RSLength.Full;

            // construct with Keyword
            _ = new RSLength(StyleKeyword.Auto);
            _ = RSLength.Auto;
            _ = RSLength.Initial;
        }
        void Operator()
        {
            RSLength len1 = 50;
            RSLength len2 = 25;

            _ = len1 + len2;
            _ = len1 - len2;
            _ = len1 * len2;
            _ = len1 / len2;

            _ = len1 + 10f;
            _ = len1 - 10f;
            _ = len1 * 10f;
            _ = len1 / 10f;

            // this equal operate will affect by ValueEqualDiff
            // if ValueEqualDiff = 0.01 means { isEqual? = abs(a-b) < Diff }, 
            // for example: [0.01, 0.011] will be equal.
            // you can change it in runtime at RSLength.ValueEqualDiff
            RSLength.ValueEqualDiff = 0.001f; // this is default value
            _ = len1 == len2;
            _ = len1 != len2;

            float  asFloat       = len1;
            Length asLength      = len1;
            Length asStyleLength = len1;

            len2 = asFloat;
            len2 = asLength;
            len2 = asStyleLength;

        }

        /* #2 ModeFlag */
        /* This is NOT a necessary part to use RSLength */
        /*-------------------------------------------------------------------|
        | RSLength include: [value, keyword, unit]                           |
        | The keyword and unit has been combind into a FlagEnum "ModeFlag"   |
        | We called it "mode". Except Keyword and Unit,                      |
        | mode will alse determine what value can be set to it on inspector  |
        | here will show you the usage and how to modify                     |
        --------------------------------------------------------------------*/
        void RSLengthModeOperate()
        {
            // The Unit, this only effect when IsUndefined is include
            // T:Pixel, F:Percent
            _ = RSLength.ModeFlag.Unit; 

            // The StyleKeyword. If None of them been set, keyword = Null
            _ = RSLength.ModeFlag.IsAuto;
            _ = RSLength.ModeFlag.IsInitial;
            _ = RSLength.ModeFlag.IsUndefined;
            _ = RSLength.ModeFlag.IsNone;

            // What value can be set to it on inspector
            // for example, is only "CanBePixel" been set,
            // you will not able to change Unit or Keyword on inspector
            // it can only be a Pixel Length (but still can be other in runtime script)
            _ = RSLength.ModeFlag.CanBePixel;
            _ = RSLength.ModeFlag.CanBePercent;
            _ = RSLength.ModeFlag.CanBeAuto;
            _ = RSLength.ModeFlag.CanBeInitial;
            _ = RSLength.ModeFlag.CanBeNone;

            _ = RSLength.ModeFlag.F_UnsetKeyword; // == ~(IsAuto  | IsInitial | IsUndefined | IsNone)
            _ = RSLength.ModeFlag.F_CanBeAny;     // ==  (CanBePixel | CanBePercent | CanBeAuto | CanBeInitial | CanBeNone)


            // --------------------------------- //
            //       Let give some example       //
            // --------------------------------- //

            // This give mode = Auto
            _ = RSLength.ModeFlag.IsAuto;

            // This give mode = Pixel
            _ = RSLength.ModeFlag.IsUndefined | RSLength.ModeFlag.Unit;

            // This give mode = Pixel, and can be set as any on Inspector
            _ = RSLength.ModeFlag.F_CanBeAny | RSLength.ModeFlag.IsUndefined | RSLength.ModeFlag.Unit;

            // This give mode = Percent, and will only be Percent on Inspector
            _ = RSLength.ModeFlag.CanBePercent | RSLength.ModeFlag.IsUndefined;

            // To ensure you know what is Mode for,
            // you can only construct RSLength by function below.
            _ = RSLength.FromMode(RSLength.ModeFlag.F_CanBeAny, 0f);
        }
    }
}
