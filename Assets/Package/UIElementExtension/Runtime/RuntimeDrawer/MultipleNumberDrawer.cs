using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public abstract class MultipleNumberDrawer<DType, NType> : RuntimeDrawer<DType>
        where DType : struct
    {
        protected abstract string value_name { get; }
        protected abstract int layout_col { get; }
        private RuntimeDrawer<NType>[] fields;
        public MultipleNumberDrawer()
        {
            fields = new RuntimeDrawer<NType>[value_name.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                fields[i] = (RuntimeDrawer<NType>)RuntimeDrawerFactory.FromValueType(typeof(NType)).Label(value_name[i].ToString()).Build();
                fields[i].labelElement.style.minWidth = StyleKeyword.Auto;
                fields[i].labelElement.style.unityTextAlign = TextAnchor.MiddleCenter;
                fields[i].labelWidth = RSTheme.Current.LineHeight;
                fields[i].iconElement.style.display = DisplayStyle.None;
                int localI = i;
                fields[i].OnValueChanged += () =>
                {
                    SetValueWithoutRepaint(Fields2DataType(fields));
                    InvokeMemberValueChange(fields[localI]);
                };
                fields[i].indentWidth = 0;
            }
            VisualElement[] row = new VisualElement[layout_col];
            for (int i = 0; i < fields.Length; i++)
            {
                if (i % layout_col == 0 && i!=0)
                {
                    Add(new RSHorizontal(row));
                    row = new VisualElement[layout_col];
                }
                row[i % layout_col] = fields[i];
            }
            for (int i = 0; i < layout_col; i++)
            {
                if (row[i] == null)
                {
                    var ve = new VisualElement();
                    ve.style.flexGrow = 1;
                    row[i] = ve;
                }
            }
            Add(new RSHorizontal(row));
        }
        protected override void CreateGUI()
        {
        }

        public override void RepaintDrawer()
        {
            DataType2Fields(fields, value);
        }
        protected abstract DType Fields2DataType(RuntimeDrawer<NType>[] fields);
        protected abstract void DataType2Fields(RuntimeDrawer<NType>[] fields, DType data);
    }

    [CustomRuntimeDrawer(typeof(Vector2), Priority = 0)]
    public class Vector2Drawer : MultipleNumberDrawer<Vector2, float>
    {
        protected override string value_name => "xy";

        protected override int layout_col => 3;

        protected override void DataType2Fields(RuntimeDrawer<float>[] fields, Vector2 data)
        {
            fields[0].SetValueWithoutNotify(data.x);
            fields[1].SetValueWithoutNotify(data.y);
        }

        protected override Vector2 Fields2DataType(RuntimeDrawer<float>[] fields)
        {
            return new Vector2(
                fields[0].value,
                fields[1].value);
        }
    }

    [CustomRuntimeDrawer(typeof(Vector2Int), Priority = 0)]
    public class Vector2IntDrawer : MultipleNumberDrawer<Vector2Int, int>
    {
        protected override string value_name => "xy";

        protected override int layout_col => 3;

        protected override void DataType2Fields(RuntimeDrawer<int>[] fields, Vector2Int data)
        {
            fields[0].SetValueWithoutNotify(data.x);
            fields[1].SetValueWithoutNotify(data.y);
        }

        protected override Vector2Int Fields2DataType(RuntimeDrawer<int>[] fields)
        {
            return new Vector2Int(
                fields[0].value,
                fields[1].value);
        }
    }

    [CustomRuntimeDrawer(typeof(Vector3), Priority = 0)]
    public class Vector3Drawer : MultipleNumberDrawer<Vector3, float>
    {
        protected override string value_name => "xyz";

        protected override int layout_col => 3;

        protected override void DataType2Fields(RuntimeDrawer<float>[] fields, Vector3 data)
        {
            fields[0].SetValueWithoutNotify(data.x);
            fields[1].SetValueWithoutNotify(data.y);
            fields[2].SetValueWithoutNotify(data.z);
        }

        protected override Vector3 Fields2DataType(RuntimeDrawer<float>[] fields)
        {
            return new Vector3(
                fields[0].value,
                fields[1].value,
                fields[2].value);
        }
    }

    [CustomRuntimeDrawer(typeof(Vector3Int), Priority = 0)]
    public class Vector3IntDrawer : MultipleNumberDrawer<Vector3Int, int>
    {
        protected override string value_name => "xyz";

        protected override int layout_col => 3;

        protected override void DataType2Fields(RuntimeDrawer<int>[] fields, Vector3Int data)
        {
            fields[0].SetValueWithoutNotify(data.x);
            fields[1].SetValueWithoutNotify(data.y);
            fields[2].SetValueWithoutNotify(data.z);
        }

        protected override Vector3Int Fields2DataType(RuntimeDrawer<int>[] fields)
        {
            return new Vector3Int(
                fields[0].value,
                fields[1].value,
                fields[2].value);
        }
    }

    [CustomRuntimeDrawer(typeof(Rect), Priority = 0)]
    public class RectDrawer : MultipleNumberDrawer<Rect, float>
    {
        protected override string value_name => "xywh";

        protected override int layout_col => 2;

        protected override void DataType2Fields(RuntimeDrawer<float>[] fields, Rect data)
        {
            fields[0].SetValueWithoutNotify(data.x);
            fields[1].SetValueWithoutNotify(data.y);
            fields[2].SetValueWithoutNotify(data.width);
            fields[3].SetValueWithoutNotify(data.height);
        }

        protected override Rect Fields2DataType(RuntimeDrawer<float>[] fields)
        {
            return new Rect(
                fields[0].value,
                fields[1].value,
                fields[2].value,
                fields[3].value);
        }
    }

    [CustomRuntimeDrawer(typeof(RectInt), Priority = 0)]
    public class RectIntDrawer : MultipleNumberDrawer<RectInt, int>
    {
        protected override string value_name => "xywh";

        protected override int layout_col => 2;

        protected override void DataType2Fields(RuntimeDrawer<int>[] fields, RectInt data)
        {
            fields[0].SetValueWithoutNotify(data.x);
            fields[1].SetValueWithoutNotify(data.y);
            fields[2].SetValueWithoutNotify(data.width);
            fields[3].SetValueWithoutNotify(data.height);
        }

        protected override RectInt Fields2DataType(RuntimeDrawer<int>[] fields)
        {
            return new RectInt(
                fields[0].value,
                fields[1].value,
                fields[2].value,
                fields[3].value);
        }
    }
}