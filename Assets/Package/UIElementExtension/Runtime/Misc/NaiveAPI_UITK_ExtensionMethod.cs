using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public static class NaiveAPI_UITK_ExtensionMethod
    {
        public static TValue GetOrCreateValue<TKey,TValue>(this Dictionary<TKey,TValue> table, TKey key)
            where TValue : new()
        {
            if(table.TryGetValue(key, out var value))
                return value;
            value = new TValue();
            table.Add(key, value);
            return value;
        }
        public static IStyle SetRS_Style(this IStyle style, RSStyleComponent component) {
            component.ApplyOn(style);
            return style;
        }
        public static IStyle ClearMarginPadding(this IStyle style)
        {
            RSMargin.Clear.ApplyOn(style);
            RSPadding.Clear.ApplyOn(style);
            return style;
        }
        public static bool IsFocusedOnPanel(this VisualElement element)
        {
            if (element.panel == null) return false;
            return element.panel.focusController.focusedElement == element;
        }
        public static Vector2 LimitPositionInParent(this VisualElement element) { return LimitPositionIn(element, element.parent); }
        public static Vector2 LimitPositionIn(this VisualElement element, VisualElement bounding)
        {
            Rect self = element.worldBound;
            Rect bound = bounding.worldBound;
            if(self.x < bound.x) self.x = bound.x;
            if (self.xMax > bound.xMax) self.x = bound.xMax - self.width;
            if (self.y < bound.y) self.y = bound.y;
            if (self.yMax > bound.yMax) self.y = bound.yMax - self.height;
            self.position = element.parent.WorldToLocal(self.position);
            element.style.left = self.x;
            element.style.top = self.y;
            return self.position;
        }

        /// <summary>
        /// Return elements from its contentContainer with only type T.
        /// </summary>
        public static IEnumerable<T> Children<T>(this VisualElement ve)
            where T : VisualElement
        {
            for(int i=0,imax = ve.contentContainer.childCount; i < imax; i++)
            {
                T ret = ve.contentContainer[i] as T;
                if(ret != null)
                    yield return ret;
            }
        }

        /// <summary>
        /// Return elements from its contentContainer in entire VisualTree.
        /// </summary>
        public static IEnumerable<VisualElement> ChildrenRecursive(this VisualElement ve, bool includeSelf = false)
        {
            if (includeSelf)
                yield return ve;
            foreach (var child in ve.Children())
            {
                yield return child;
                foreach(var childChild in ChildrenRecursive(child))
                    yield return childChild;
            }
        }
        /// <summary>
        /// Return elements from its contentContainer in entire VisualTree.
        /// </summary>
        public static IEnumerable<VisualElement> HierarchyRecursive(this VisualElement ve, bool includeSelf = false)
        {
            if (includeSelf)
                yield return ve;
            foreach (var child in ve.hierarchy.Children())
            {
                yield return child;
                foreach (var childChild in HierarchyRecursive(child))
                    yield return childChild;
            }
        }
        /// <summary>
        /// Return elements from its contentContainer in entire VisualTree with only type T.
        /// </summary>
        public static IEnumerable<T> ChildrenRecursive<T>(this VisualElement ve, bool includeSelf = false)
            where T : VisualElement
        {
            if(includeSelf)
            {
                var self = ve as T;
                if (self != null)
                    yield return self;
            }
            foreach(var element in ve.ChildrenRecursive())
            {
                T ret = element as T;
                if(ret != null)
                    yield return ret;
            }
        }
        /// <summary>
        /// Return elements from its contentContainer in entire VisualTree with only type T.
        /// </summary>
        public static IEnumerable<T> HierarchyRecursive<T>(this VisualElement ve, bool includeSelf = false)
            where T : VisualElement
        {
            if (includeSelf)
            {
                var self = ve as T;
                if (self != null)
                    yield return self;
            }
            foreach (var element in ve.HierarchyRecursive())
            {
                T ret = element as T;
                if (ret != null)
                    yield return ret;
            }
        }

        [ThreadStatic]
        private static int[,] levenshteinDistanceDP;
        public static int LevenshteinDistance(this string lhs, string rhs)
        {
            if ((levenshteinDistanceDP == null) ? true : (levenshteinDistanceDP.GetLength(0) < (lhs.Length + 1) || levenshteinDistanceDP.GetLength(1) < (rhs.Length + 1)))
            {
                levenshteinDistanceDP = new int[Math.Max(levenshteinDistanceDP?.GetLength(0) ?? 0, lhs.Length + 1), Math.Max(levenshteinDistanceDP?.GetLength(1) ?? 0, rhs.Length + 1)];
                levenshteinDistanceDP[0, 0] = 0;
            }

            for (int i = 0; i < lhs.Length; i++)
                levenshteinDistanceDP[i + 1, 0] = i + 1;
            for (int i = 0; i < rhs.Length; i++)
                levenshteinDistanceDP[0, i + 1] = i + 1;
            for (int y = 0; y < lhs.Length; y++)
            {
                for (int x = 0; x < rhs.Length; x++)
                {
                    var val = Math.Min(Math.Min(levenshteinDistanceDP[y, x], levenshteinDistanceDP[y + 1, x]), levenshteinDistanceDP[y, x + 1]);
                    if (lhs[y] != rhs[x]) val++;
                    levenshteinDistanceDP[y + 1, x + 1] = val;
                }
            }
            return levenshteinDistanceDP[lhs.Length, rhs.Length];
        }
        public static int LevenshteinDistanceBasedCost(this string lhs, string rhs, float containsMult = 0.5f,int maxLenDiff = int.MaxValue)
        {
            var ret = 0;
            if (Math.Abs(lhs.Length - rhs.Length) < maxLenDiff)
                ret = LevenshteinDistance(lhs, rhs);
            else
                ret = lhs.Length + rhs.Length;
            if (lhs.Contains(rhs, StringComparison.Ordinal))
                ret *= (int)containsMult;
            return ret;
        }
    }
}
