using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public abstract class StandardDrawer<T> : FoldoutDrawer<T>
    {
        protected Action updateAction;
        protected RuntimeDrawer AddDrawer<V>(string label, Func<V> getter, Action<V> setter, IEnumerable<Attribute> attributes = null)
        { return AddDrawer(label, typeof(V), getter, setter, attributes); }
        protected RuntimeDrawer AddDrawer<V>(string label, Type type, Func<V> getter, Action<V> setter, IEnumerable<Attribute> attributes = null)
        {
            RuntimeDrawer drawer = RuntimeDrawerFactory.FromValueType(type).Label(DefaultDrawer.ProcessName(label)).AddAttribute(attributes).Build();
            AddDrawer(drawer, getter, setter);
            return drawer;
        }
        protected void AddDrawer<V>(RuntimeDrawer drawer, Func<V> getter, Action<V> setter)
        {
            drawer.OnValueChanged += () =>
            {
                setter((V)drawer.GetValue());
                InvokeMemberValueChange(drawer);
            };
            drawer.OnMemberValueChanged += InvokeMemberValueChange;
            updateAction += () => { drawer.SetValueWithoutNotify(getter()); };
            Add(drawer);
        }
        public override void RepaintDrawer()
        {
            if (!FoldoutState) return;
            if (value == null) return;
            updateAction?.Invoke();
        }
        protected void ClearDrawer()
        {
            Clear();
            updateAction = null;
        }
    }
}