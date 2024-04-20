using NaiveAPI.UITK;
using UnityEditor;
using UnityEngine;

namespace NaiveAPI_Editor.UITK
{
    [CustomPropertyDrawer(typeof(RSRadius))]
    public class RSRadiusPropertyDrawer : RSStyleComponentPropertyDrawer<RSRadius>
    {
        public override void DecodeValueFromProperty(SerializedProperty property)
        {
            var prop = property.Copy();
            prop.Next(true);
            value.SetUnsetFlag = prop.intValue;
            prop.Next(false);
            value.topLeft = RSLengthPropertyDrawer.ParseFromSerializedProperty(prop);
            prop.Next(false);
            value.bottomLeft = RSLengthPropertyDrawer.ParseFromSerializedProperty(prop);
            prop.Next(false);
            value.topRight = RSLengthPropertyDrawer.ParseFromSerializedProperty(prop);
            prop.Next(false);
            value.bottomRight = RSLengthPropertyDrawer.ParseFromSerializedProperty(prop);
        }

        public override void EncodeValueToProperty(SerializedProperty property)
        {
            var prop = property.Copy();
            prop.Next(true);
            prop.intValue = value.SetUnsetFlag;
            prop.Next(false);
            RSLengthPropertyDrawer.ApplyOnSerializedProperty(value.topLeft, prop);
            prop.Next(false);
            RSLengthPropertyDrawer.ApplyOnSerializedProperty(value.bottomLeft, prop);
            prop.Next(false);
            RSLengthPropertyDrawer.ApplyOnSerializedProperty(value.topRight, prop);
            prop.Next(false);
            RSLengthPropertyDrawer.ApplyOnSerializedProperty(value.bottomRight, prop);
        }

        RSLengthPropertyDrawer[] lenDrawers = new RSLengthPropertyDrawer[] { new(), new(), new(), new() };

        public override int GetRenderHeight()
        {
            return 20 * 4 + 5;
        }

        public override void OnGUI(Rect position)
        {
            position.height = 18; position.y++;
            BeginSetUnsetFieldByIndex(position, 0);
            value.topLeft = lenDrawers[0].OnGUI(position, value.topLeft, new GUIContent("TopLeft"));
            EndSetUnsetField(); position.y += 20;
            BeginSetUnsetFieldByIndex(position, 1);
            value.bottomLeft = lenDrawers[1].OnGUI(position, value.bottomLeft, new GUIContent("BottomLeft"));
            EndSetUnsetField(); position.y += 20;
            BeginSetUnsetFieldByIndex(position, 2);
            value.topRight = lenDrawers[2].OnGUI(position, value.topRight, new GUIContent("TopRight"));
            EndSetUnsetField(); position.y += 20;
            BeginSetUnsetFieldByIndex(position, 3);
            value.bottomRight = lenDrawers[3].OnGUI(position, value.bottomRight, new GUIContent("BottomRight"));
            EndSetUnsetField();
        }
    }
}
