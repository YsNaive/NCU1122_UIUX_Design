using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public class RSButton : Button
    {
        public new class UxmlFactory : UxmlFactory<RSButton, TextElement.UxmlTraits> {}
        public Color BackgroundColor
        {
            get => m_BackgroundColor;
            set
            {
                style.backgroundColor = value;
                m_BackgroundColor = value;
            }
        }
        public Color HoverColor
        {
            get => m_HoverColor;
            set => m_HoverColor = value;
        }
        Color m_BackgroundColor;
        Color m_HoverColor;

        public RSButton() : this("", RSTheme.Current.NormalColorSet, null) { }
        public RSButton(string text, Action clicked = null) : this(text, RSTheme.Current.NormalColorSet, clicked) { }
        public RSButton(string text, RSColorSet colorSet, Action clicked = null)
        {
            if (clicked != null)
                this.clicked += clicked;
            this.text = text;

            RegisterCallback<PointerEnterEvent>(e => { style.backgroundColor = m_HoverColor; });
            RegisterCallback<PointerLeaveEvent>(e => { style.backgroundColor = m_BackgroundColor; });

            var theme = RSTheme.Current;
            RSPadding.op_temp.any = 0;
            RSPadding.op_temp.left = theme.VisualMargin;
            RSPadding.op_temp.right = theme.VisualMargin;
            RSMargin.op_temp.any = theme.VisualMargin / 4f;
            RSBorder.op_temp.anyWidth = theme.VisualMargin / 4f;
            RSBorder.op_temp.anyColor = colorSet.BackgroundColor3;
            style.SetRS_Style(RSPadding.op_temp);
            style.SetRS_Style(RSMargin.op_temp);
            style.SetRS_Style(RSBorder.op_temp);
            style.minHeight = theme.LineHeight - theme.VisualMargin;
            style.color = colorSet.TextColor;
            HoverColor = colorSet.BackgroundColor3;
            BackgroundColor = colorSet.BackgroundColor2;

            style.backgroundColor = m_BackgroundColor;
        }
    }
}
