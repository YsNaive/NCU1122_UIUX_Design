using NaiveAPI.UITK;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NaiveAPI_Editor.UITK
{
    [CustomPropertyDrawer(typeof(RSStyle))]
    public class RSStylePropertyDrawer : RSStyleComponentPropertyDrawer<RSStyle>
    {
        RSDisplayPropertyDrawer displayDrawer = new();
        RSPositionPropertyDrawer positionDrawer = new();
        RSFlexPropertyDrawer flexDrawer = new();
        RSAlignPropertyDrawer alignDrawer = new();
        RSSizePropertyDrawer sizeDrawer = new();
        RSMarginPropertyDrawer marginDrawer = new();
        RSPaddingPropertyDrawer paddingDrawer = new();
        RSTextPropertyDrawer textDrawer = new();
        RSBackgroundPropertyDrawer backgroundDrawer = new();
        RSBorderPropertyDrawer borderDrawer = new();
        RSRadiusPropertyDrawer radiusDrawer = new();
        RSTransformPropertyDrawer transformDrawer = new();
        public RSStylePropertyDrawer()
        {
            Func<SerializedProperty> get = () =>
            {
                return OnGUIProperty.Copy();
            };
            Action<SerializedProperty> set = (prop) =>
            {
                SaveOn(prop);
            };

            displayDrawer.GetGenericMenuAffectProperty = get;
            displayDrawer.GenericMenuSaveProperty = set;
            positionDrawer.GetGenericMenuAffectProperty = get;
            positionDrawer.GenericMenuSaveProperty = set;
            flexDrawer.GetGenericMenuAffectProperty = get;
            flexDrawer.GenericMenuSaveProperty = set;
            alignDrawer.GetGenericMenuAffectProperty = get;
            alignDrawer.GenericMenuSaveProperty = set;
            sizeDrawer.GetGenericMenuAffectProperty = get;
            sizeDrawer.GenericMenuSaveProperty = set;
            marginDrawer.GetGenericMenuAffectProperty = get;
            marginDrawer.GenericMenuSaveProperty = set;
            paddingDrawer.GetGenericMenuAffectProperty = get;
            paddingDrawer.GenericMenuSaveProperty = set;
            textDrawer.GetGenericMenuAffectProperty = get;
            textDrawer.GenericMenuSaveProperty = set;
            backgroundDrawer.GetGenericMenuAffectProperty = get;
            backgroundDrawer.GenericMenuSaveProperty = set;
            borderDrawer.GetGenericMenuAffectProperty = get;
            borderDrawer.GenericMenuSaveProperty = set;
            radiusDrawer.GetGenericMenuAffectProperty = get;
            radiusDrawer.GenericMenuSaveProperty = set;
            transformDrawer.GetGenericMenuAffectProperty = get;
            transformDrawer.GenericMenuSaveProperty = set;
        }
        void applyDrawerValueFromSelfValue()
        {
            displayDrawer.value = value.Display;
            positionDrawer.value = value.Position;
            flexDrawer.value = value.Flex;
            alignDrawer.value = value.Align;
            sizeDrawer.value = value.Size;
            marginDrawer.value = value.Margin;
            paddingDrawer.value = value.Padding;
            textDrawer.value = value.Text;
            backgroundDrawer.value = value.Background;
            borderDrawer.value = value.Border;
            radiusDrawer.value = value.Radius;
            transformDrawer.value = value.Transform;
        }
        public void DecodeExpandedValueFromProperty(SerializedProperty property)
        {
            OnGUIDataProperty    = property.FindPropertyRelative("s_data").FindPropertyRelative("s_data");
            OnGUIObjDataProperty = property.FindPropertyRelative("s_data").FindPropertyRelative("s_objData");
            int i = 0;
            int flag = property.FindPropertyRelative("m_flag").intValue;
            value.SetUnsetFlag = flag;
            if(RSStyleComponent.GetFlag((int)RSStyleFlag.Display   , flag)) displayDrawer.IsExpanded =    OnGUIDataProperty.GetArrayElementAtIndex(i++).isExpanded;
            if(RSStyleComponent.GetFlag((int)RSStyleFlag.Position  , flag)) positionDrawer.IsExpanded =   OnGUIDataProperty.GetArrayElementAtIndex(i++).isExpanded;
            if(RSStyleComponent.GetFlag((int)RSStyleFlag.Flex      , flag)) flexDrawer.IsExpanded =       OnGUIDataProperty.GetArrayElementAtIndex(i++).isExpanded;
            if(RSStyleComponent.GetFlag((int)RSStyleFlag.Align     , flag)) alignDrawer.IsExpanded =      OnGUIDataProperty.GetArrayElementAtIndex(i++).isExpanded;
            if(RSStyleComponent.GetFlag((int)RSStyleFlag.Size      , flag)) sizeDrawer.IsExpanded =       OnGUIDataProperty.GetArrayElementAtIndex(i++).isExpanded;
            if(RSStyleComponent.GetFlag((int)RSStyleFlag.Margin    , flag)) marginDrawer.IsExpanded =     OnGUIDataProperty.GetArrayElementAtIndex(i++).isExpanded;
            if(RSStyleComponent.GetFlag((int)RSStyleFlag.Padding   , flag)) paddingDrawer.IsExpanded =    OnGUIDataProperty.GetArrayElementAtIndex(i++).isExpanded;
            if(RSStyleComponent.GetFlag((int)RSStyleFlag.Text      , flag)) textDrawer.IsExpanded =       OnGUIDataProperty.GetArrayElementAtIndex(i++).isExpanded;
            if(RSStyleComponent.GetFlag((int)RSStyleFlag.Background, flag)) backgroundDrawer.IsExpanded = OnGUIDataProperty.GetArrayElementAtIndex(i++).isExpanded;
            if(RSStyleComponent.GetFlag((int)RSStyleFlag.Border    , flag)) borderDrawer.IsExpanded =     OnGUIDataProperty.GetArrayElementAtIndex(i++).isExpanded;
            if(RSStyleComponent.GetFlag((int)RSStyleFlag.Radius    , flag)) radiusDrawer.IsExpanded =     OnGUIDataProperty.GetArrayElementAtIndex(i++).isExpanded;
            if(RSStyleComponent.GetFlag((int)RSStyleFlag.Transform , flag)) transformDrawer.IsExpanded =  OnGUIDataProperty.GetArrayElementAtIndex(i++).isExpanded;
        }
        public override void DecodeValueFromProperty(SerializedProperty property)
        {
            DecodeExpandedValueFromProperty(property);
            List<int> data = new List<int>();
            List<Object> objData = new List<Object>();
            int i = 0;
            for (i = 0; i < OnGUIDataProperty.arraySize; i++)
                data.Add(OnGUIDataProperty.GetArrayElementAtIndex(i).intValue);
            for (i = 0; i < OnGUIObjDataProperty.arraySize; i++)
                objData.Add(OnGUIObjDataProperty.GetArrayElementAtIndex(i).objectReferenceValue);
            var handle = new RSStyleCompressSerializer() { s_data = data, s_objData = objData };
            handle.UnZip();
            value.LoadFrom((RSStyle)handle.StyleComponents[0]);
            applyDrawerValueFromSelfValue();
        }
        public override void EncodeValueToProperty(SerializedProperty property)
        {
            OnGUIDataProperty = property.FindPropertyRelative("s_data").FindPropertyRelative("s_data");
            OnGUIObjDataProperty = property.FindPropertyRelative("s_data").FindPropertyRelative("s_objData");
            var handle = new RSStyleCompressSerializer() { StyleComponents = new() { value } };
            handle.Zip();
            OnGUIDataProperty.arraySize = handle.s_data.Count;
            for (int i = 0; i < handle.s_data.Count; i++)
            {
                OnGUIDataProperty.GetArrayElementAtIndex(i).intValue = handle.s_data[i];
            }

            OnGUIObjDataProperty.arraySize = handle.s_objData.Count;
            for (int i = 0; i < handle.s_objData.Count; i++)
                OnGUIObjDataProperty.GetArrayElementAtIndex(i).objectReferenceValue = handle.s_objData[i];
        }
        protected override Rect BeforeRenderHeader(Rect headerRect, GUIContent label)
        {
            var orgText = label.text;
            var maskRect = headerRect;
            maskRect.y--;
            maskRect.xMin = maskRect.xMax - 98;
            var flagValue = value.SetUnsetFlag;
            EditorGUI.BeginChangeCheck();
            flagValue = Convert.ToInt32(EditorGUI.EnumFlagsField(maskRect, (RSStyleFlag)flagValue));
            if (EditorGUI.EndChangeCheck())
            {
                value.SetUnsetFlag = flagValue;
                SaveOn(OnGUIProperty);

                applyDrawerValueFromSelfValue();
            }

            headerRect.xMax -= 100;
            label.text = orgText;
            return headerRect;
        }

        SerializedProperty OnGUIDataProperty;
        SerializedProperty OnGUIObjDataProperty;
        private Rect OnDrawerGUI<DrawerType, RSType>(Rect position, DrawerType drawer, string label,ref int index)
            where RSType     : RSStyleComponent<RSType>, new()
            where DrawerType : RSStyleComponentPropertyDrawer<RSType>
        {
            if(drawer.value != null)
            {
                position.height = drawer.GetPropertyHeight();
                drawer.IsExpanded = OnGUIDataProperty.GetArrayElementAtIndex(index).isExpanded;
                drawer.OnGUI(position, new GUIContent(label));
                OnGUIDataProperty.GetArrayElementAtIndex(index).isExpanded = drawer.IsExpanded;
                position.y += position.height;
                index++;
            }
            return position;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);
        }
        public override void OnGUI(Rect position)
        {
            int i = 0;
            position.xMin += 15;
            position = OnDrawerGUI<RSDisplayPropertyDrawer, RSDisplay>(position, displayDrawer, "Display", ref i);
            position = OnDrawerGUI<RSPositionPropertyDrawer, RSPosition>(position, positionDrawer, "Position", ref i);
            position = OnDrawerGUI<RSFlexPropertyDrawer, RSFlex>(position, flexDrawer, "Flex", ref i);
            position = OnDrawerGUI<RSAlignPropertyDrawer, RSAlign>(position, alignDrawer, "Align", ref i);
            position = OnDrawerGUI<RSSizePropertyDrawer, RSSize>(position, sizeDrawer, "Size", ref i);
            position = OnDrawerGUI<RSMarginPropertyDrawer, RSMargin>(position, marginDrawer, "Margin", ref i);
            position = OnDrawerGUI<RSPaddingPropertyDrawer, RSPadding>(position, paddingDrawer, "Padding", ref i);
            position = OnDrawerGUI<RSTextPropertyDrawer, RSText>(position, textDrawer, "Text", ref i);
            position = OnDrawerGUI<RSBackgroundPropertyDrawer, RSBackground>(position, backgroundDrawer, "Background", ref i);
            position = OnDrawerGUI<RSBorderPropertyDrawer, RSBorder>(position, borderDrawer, "Border", ref i);
            position = OnDrawerGUI<RSRadiusPropertyDrawer, RSRadius>(position, radiusDrawer, "Radius", ref i);
            position = OnDrawerGUI<RSTransformPropertyDrawer, RSTransform>(position, transformDrawer, "Transform", ref i);

            IsDirty =
                displayDrawer.   IsDirty |
                positionDrawer.  IsDirty |
                flexDrawer.      IsDirty |
                sizeDrawer.      IsDirty |
                alignDrawer.     IsDirty |
                marginDrawer.    IsDirty |
                paddingDrawer.   IsDirty |
                textDrawer.      IsDirty |
                backgroundDrawer.IsDirty |
                borderDrawer.    IsDirty |
                radiusDrawer.    IsDirty |
                transformDrawer. IsDirty ;
        }

        public override int GetRenderHeight()
        {
            DecodeExpandedValueFromProperty(OnGUIProperty);
            float sum = 0;
            sum += value.GetEnable(RSStyleFlag.Display)    ? displayDrawer   .GetPropertyHeight() : 0;
            sum += value.GetEnable(RSStyleFlag.Position)   ? positionDrawer  .GetPropertyHeight() : 0;
            sum += value.GetEnable(RSStyleFlag.Flex)       ? flexDrawer      .GetPropertyHeight() : 0;
            sum += value.GetEnable(RSStyleFlag.Align)      ? alignDrawer     .GetPropertyHeight() : 0;
            sum += value.GetEnable(RSStyleFlag.Size)       ? sizeDrawer      .GetPropertyHeight() : 0;
            sum += value.GetEnable(RSStyleFlag.Margin)     ? marginDrawer    .GetPropertyHeight() : 0;
            sum += value.GetEnable(RSStyleFlag.Padding)    ? paddingDrawer   .GetPropertyHeight() : 0;
            sum += value.GetEnable(RSStyleFlag.Text)       ? textDrawer      .GetPropertyHeight() : 0;
            sum += value.GetEnable(RSStyleFlag.Background) ? backgroundDrawer.GetPropertyHeight() : 0;
            sum += value.GetEnable(RSStyleFlag.Border)     ? borderDrawer    .GetPropertyHeight() : 0;
            sum += value.GetEnable(RSStyleFlag.Radius)     ? radiusDrawer    .GetPropertyHeight() : 0;
            sum += value.GetEnable(RSStyleFlag.Transform)  ? transformDrawer .GetPropertyHeight() : 0;
            return (int)sum;
        }

    }
}
