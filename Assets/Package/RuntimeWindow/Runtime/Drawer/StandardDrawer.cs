using NaiveAPI.DocumentBuilder;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.RuntimeWindowUtils
{
    public abstract class StandardDrawer<T> : FoldoutDrawer<T>
    {
        protected Action updateAction;
        protected void AddDrawer<V>(string label, Func<V> getter, Action<V> setter, IEnumerable<Attribute> attributes = null)
        {
            RuntimeDrawer drawer = RuntimeDrawerFactory.FromValueType(typeof(V)).Label(label).AddAttribute(attributes).Build();
            AddDrawer(drawer, getter, setter);
        }
        protected void AddDrawer<V>(RuntimeDrawer drawer, Func<V> getter, Action<V> setter)
        {
            drawer.RegisterValueChangedAsEventBase(evt =>
            {
                setter((V)drawer.GetValue());
                evt.StopPropagation();
            });
            updateAction += () => { drawer.SetValueWithoutNotify(getter()); };
            Add(drawer);
        }
        public override void UpdateField()
        {
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