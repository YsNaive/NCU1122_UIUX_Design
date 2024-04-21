using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public class TypeDrawer : RuntimeDrawer<Type>
    {
        class SearchInfo
        {
            public Type type;
            public string searchName;
        }
        static SearchInfo[] searchInfos;
        static event Action<Type> callback = (_) => { opendTooltip?.Close(); };
        static Dictionary<Type,VisualElement> choicesVisual = new();
        static SearchView<string, Type> searchView;
        static Func<Type, bool> currentFilter;
        static TooltipElement opendTooltip;
        static TypeDrawer()
        {
            searchInfos = new SearchInfo[TypeReader.ActiveTypes.Count];
            int i = 0;
            foreach (var type in TypeReader.ActiveTypes)
            {
                SearchInfo info = new SearchInfo();
                info.type = type;
                info.searchName = TypeReader.GetName(type);
                int subLength = info.searchName.LastIndexOf('<');
                if(subLength > 0)
                    info.searchName = info.searchName.Substring(0, subLength);
                searchInfos[i++] = info;
            }

            searchView = new SearchView<string, Type>(
                (key) => searchInfos
                .Where(info =>{ return currentFilter?.Invoke(info.type) ?? true;})
                .OrderBy(info =>
                    {
                        var distance = info.searchName.LevenshteinDistance(key);
                        var lenDiff = Math.Abs(info.searchName.Length - key.Length);
                        if (lenDiff > 8)
                            return info.searchName.Length + key.Length;
                        if (info.searchName.StartsWith(key, StringComparison.OrdinalIgnoreCase))
                            distance /= 2;
                        if (info.searchName.Contains(key, StringComparison.OrdinalIgnoreCase))
                            distance /= 2;
                        return distance;
                    })
                .Select(info => info.type),
                (type) =>
                {
                    var visual = new RSTypeNameElement(type);
                    visual.style.paddingLeft = RSTheme.Current.LineHeight / 2f;
                    visual.style.paddingRight = visual.style.paddingLeft;
                    visual.style.borderBottomColor = RSTheme.Current.BackgroundColor2;
                    visual.style.borderBottomWidth = 1;
                    visual.style.flexShrink = 0;
                    visual.style.unityTextAlign = TextAnchor.MiddleLeft;
                    visual.RegisterCallback<PointerEnterEvent>(evt => { visual.style.backgroundColor = RSTheme.Current.BackgroundColor2; });
                    visual.RegisterCallback<PointerLeaveEvent>(evt => { visual.style.backgroundColor = Color.clear; });

                    var tooltipElement = new TooltipElement(visual);
                    tooltipElement.style.backgroundColor = RSTheme.Current.BackgroundColor;
                    tooltipElement.style.SetRS_Style(new RSBorder { anyColor = RSTheme.Current.FrontgroundColor, anyWidth = 1f });
                    tooltipElement.style.paddingLeft = RSTheme.Current.VisualMargin;
                    tooltipElement.style.paddingRight = RSTheme.Current.VisualMargin;
                    tooltipElement.Add(new RSTextElement("{ } " + type.Namespace) { style = { unityTextAlign = TextAnchor.MiddleLeft } });
                    tooltipElement.PopupDelay = 350;
                    tooltipElement.RegisterPopupOnTarget(visual);
                    tooltipElement.OnOpend += (_) => { opendTooltip?.Close(); opendTooltip = tooltipElement; };
                    choicesVisual.Add(type, visual);
                    visual.RegisterCallback<PointerDownEvent>(evt =>
                    {
                        callback?.Invoke(type);
                        tooltipElement.Close();
                    });
                    return visual;
                });
        }
        RSTextField searchField;
        PopupElement choicesPopup;
        public Func<Type, bool> ChoicesFilter;
        public override void RepaintDrawer()
        {
            Sprite img = RSTheme.Current.CSharp.cSharpIcon;
            string text = "-";
            if (value != null)
            {
                text = TypeReader.GetName(value);
                img = RSTheme.Current.CSharp.GetMemberIcon(value);
            }
            searchField.SetValueWithoutNotify(text);
            iconElement.style.backgroundImage = Background.FromSprite(img);
        }

        public int MaxDisplayCount { get => searchView.MaxDisplayCount; set => searchView.MaxDisplayCount = value; }

        protected override void CreateGUI()
        {
            choicesPopup = new();
            choicesPopup.Add(searchView);
            choicesPopup.OnClosed += () =>
            {
                callback -= onSelected;
                RepaintDrawer();
            };
            choicesPopup.OnOpend += _ =>
            {
                currentFilter = ChoicesFilter;
                searchView.Search("");
            };

            choicesPopup.style.backgroundColor = RSTheme.Current.BackgroundColor;
            choicesPopup.style.SetRS_Style(new RSBorder(RSTheme.Current.FrontgroundColor, 1));
            choicesPopup.style.maxHeight = Length.Percent(65);
            choicesPopup.style.minWidth = RSTheme.Current.LabelWidth * 1.5f;

            searchField = new();
            searchField.style.flexGrow = 1;
            searchField.style.minHeight = RSTheme.Current.LineHeight;
            Add(searchField);
            searchField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue != evt.previousValue)
                {
                    if (!choicesPopup.IsOpend)
                    {
                        callback += onSelected;
                        choicesPopup.OpenBelow(searchField);
                    }
                    searchView.Search(evt.newValue);
                }
                evt.StopImmediatePropagation();
            });
            searchField.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (!choicesPopup.IsOpend)
                {
                    callback += onSelected;
                    choicesPopup.OpenBelow(searchField);
                }
                searchField.schedule.Execute(searchField.SelectAll).ExecuteLater(10);
            });
            var icon = RSTheme.Current.CreateIconElement(RSTheme.Current.Icon.arrow, 90);
            icon.style.right = 0;
            icon.style.left = StyleKeyword.Auto;
            icon.style.paddingLeft = StyleKeyword.Auto;
            icon.style.marginLeft = StyleKeyword.Auto;
            searchField[0].Add(icon);
            RepaintDrawer();
        }
        void onSelected(Type evt)
        {
            callback -= onSelected;
            SetValueWithoutRepaint(evt);
            searchField.SelectRange(0,0);
            choicesPopup.Close();
        }
    }
}
