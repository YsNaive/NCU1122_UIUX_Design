using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public static class RSContextMenu
    {
        private class MenuContainer : PopupElement
        {
            public event Action<string> OnSelected;
            public List<MenuContainer> ChildMenuContainer = new();
            public MenuContainer(node node, Action<string> callback = null)
            {
                if (callback != null)
                    OnSelected += callback;
                OnSelected += str => { Close(); };
                style.minWidth = RSTheme.Current.LabelWidth;
                style.backgroundColor = RSTheme.Current.BackgroundColor;
                style.SetRS_Style(new RSBorder(RSTheme.Current.FrontgroundColor, 1));
                var container = new RSScrollView();

                EventCallback<GeometryChangedEvent> registerMaxSize = null; registerMaxSize = evt =>
                {
                    container.style.maxHeight = container.panel.visualTree.worldBound.height * 0.75f;
                    container.UnregisterCallback(registerMaxSize);
                }; container.RegisterCallback(registerMaxSize);

                Add(container);

                foreach (var choice in node.choices)
                {
                    var textElement = createMenuText(choice);
                    textElement.RegisterCallback<PointerDownEvent>(evt =>
                    { OnSelected?.Invoke($"{(node.path == "" ? "" : $"{node.path}/")}{choice}"); });
                    textElement.RegisterCallback<PointerEnterEvent>(evt => { textElement.style.backgroundColor = RSTheme.Current.BackgroundColor2; });
                    textElement.RegisterCallback<PointerLeaveEvent>(evt => { textElement.style.backgroundColor = Color.clear; });
                    textElement.RegisterCallback<PointerDownEvent>(evt => { textElement.style.backgroundColor = Color.clear; });
                    container.Add(textElement);
                }

                if (node.children != null)
                {
                    foreach (var child in node.children)
                    {
                        var title = createSubMenu(child);
                        container.Add(title);
                    }
                }

                AutoClose = true;
            }
            RSTextElement createMenuText(string text)
            {
                var textElement = new RSTextElement(text);
                textElement.style.paddingLeft = RSTheme.Current.LineHeight / 2f;
                textElement.style.paddingRight = RSTheme.Current.LineHeight / 2f;
                textElement.style.paddingTop = textElement.style.paddingBottom = 2;
                textElement.style.borderBottomWidth = 1.5f;
                textElement.style.borderBottomColor = RSTheme.Current.BackgroundColor2;
                textElement.style.whiteSpace = WhiteSpace.NoWrap;
                //textElement.style.flexGrow = 1;
                textElement.style.flexShrink = 0;
                return textElement;
            }
            RSHorizontal createSubMenu(node node)
            {
                var title = createMenuText(node.value);
                var arrow = RSTheme.Current.CreateIconElement(RSTheme.Current.Icon.arrow);
                arrow.style.marginLeft = StyleKeyword.Auto;
                var hor = new RSHorizontal();
                hor.Add(title);
                hor.Add(arrow);
                hor.RegisterCallback<PointerEnterEvent>(evt => { hor.style.backgroundColor = RSTheme.Current.BackgroundColor2; });
                hor.RegisterCallback<PointerLeaveEvent>(evt => { hor.style.backgroundColor = Color.clear; });
                hor.RegisterCallback<PointerDownEvent>(evt => { hor.style.backgroundColor = Color.clear; });
                var childMenu = new MenuContainer(node, OnSelected);
                childMenu.CoverMask.pickingMode = PickingMode.Ignore;
                OnClosed += childMenu.Close;
                bool isInside = false;
                hor.RegisterCallback<PointerEnterEvent>(evt =>
                {
                    isInside = true;
                    hor.schedule.Execute(() =>
                    {
                        if (!isInside) return;
                        foreach (var child in ChildMenuContainer)
                            child.Close();
                        childMenu.OpenBelow(this);
                        EventCallback<GeometryChangedEvent> changeEvt = null;
                        changeEvt = evt =>
                        {
                            var pos = new Vector2(0, hor.worldBound.y - 1);
                            if (childMenu.parent.worldBound.xMax - hor.worldBound.xMax > childMenu.resolvedStyle.width)
                            {
                                pos.x = hor.worldBound.xMax;
                            }
                            else
                            {
                                pos.x = hor.worldBound.x - childMenu.resolvedStyle.width;
                                pos.y += RSTheme.Current.LineHeight / 2f;
                            }

                            pos = childMenu.parent.WorldToLocal(pos);
                            childMenu.style.left = pos.x;
                            childMenu.style.top = pos.y;
                            childMenu.UnregisterCallback(changeEvt);
                        }; childMenu.RegisterCallback(changeEvt);
                    }).ExecuteLater(150);
                });
                childMenu.RegisterCallback<PointerLeaveEvent>(evt =>
                {
                    foreach (var child in childMenu.ChildMenuContainer)
                        if (child.IsOpend) return;
                    childMenu.Close();
                });
                title.RegisterCallback<PointerLeaveEvent>(evt =>
                {
                    isInside = false;
                });
                ChildMenuContainer.Add(childMenu);
                return hor;
            }
        }
        private class node
        {
            public string path = "";
            public string value = "";
            public List<node> children;
            public List<string> choices = new();
            public void AddChoice(string value)
            {
                choices.Add(value);
            }
            public void AddChild(string value)
            {
                children ??= new();
                children.Add(new node { value = value, path = $"{(path == "" ? "" : $"{path}/")}{value}" });
            }
        }
        private static node parseChoices(List<string> choices)
        {
            node rootNode = new node() { value = "" };
            foreach (var choice in choices)
            {
                var path = choice.Split('/', StringSplitOptions.RemoveEmptyEntries);
                node current = rootNode;
                for (int i = 0, imax = path.Length - 1; i < imax; i++)
                {
                    bool found = false;
                    if (current.children != null)
                    {
                        foreach (var n in current.children)
                        {
                            if (n.value == path[i])
                            {
                                found = true;
                                current = n;
                                break;
                            }
                        }
                    }
                    if (found) continue;
                    current.AddChild(path[i]);
                    current = current.children[^1];
                }
                current.AddChoice(path[^1]);
            }
            return rootNode;
        }
        public static PopupElement CreatePopupMenu(List<string> choices, Action<string> callback)
        {
            return new MenuContainer(parseChoices(choices), callback);
        }
        public static void Open(IPanel panel, Vector2 position, List<string> choices, Action<string> callback)
        {
            PopupElement popup = CreatePopupMenu(choices, callback);
            popup.OpenBelow(panel.visualTree);
            position = popup.parent.WorldToLocal(position);
            popup.style.left = position.x;
            popup.style.top = position.y;
        }
    }
}
