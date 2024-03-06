using NaiveAPI.DocumentBuilder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.RuntimeWindowUtils
{
    [CustomRuntimeDrawer(typeof(object), Priority = -99, DrawAssignableType = true)]
    public sealed class DefaultDrawer : DefaultDrawer<object>
    {
        public override bool FieldFilter(FieldInfo info)
        {
            if (info.FieldType.IsAssignableFrom(value.GetType())) 
                return false;
            if (info.Name.StartsWith("m_") || info.Name.StartsWith("s_"))
                return false;
            if (info.DeclaringType != value.GetType())
                return false;
            var isSystemSerialize = info.FieldType.IsDefined(typeof(SerializableAttribute));
            var isUnitySerialize = info.IsPublic ? !info.IsDefined(typeof(HideInInspector), false) : info.IsDefined(typeof(SerializeField), false);
            return isSystemSerialize && isUnitySerialize;
        }
    }
    public abstract class DefaultDrawer<T> : StandardDrawer<T>
    {
        public static Dictionary<MemberInfo, Func<object, object>> GetterDict = new();
        public static Dictionary<MemberInfo, Action<object, object>> SetterDict = new();
        public override bool DynamicLayout => true;

        protected override void OnCreateGUI()
        {
            ClearDrawer();
            if (value == null) return;
            foreach (var info in
                value.GetType()
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(FieldFilter))
            {
                if (!GetterDict.TryGetValue(info, out var getter))
                {
#if false
                    var parameter = Expression.Parameter(typeof(object));
                    var memberExpression = member switch
                    {
                        FieldInfo    info => Expression.Field   (Expression.Convert(parameter, info.DeclaringType), info),
                        PropertyInfo info => Expression.Property(Expression.Convert(parameter, info.DeclaringType), info),
                        _ => null
                    };
                    getter = Expression.Lambda<Func<object, object>>(Expression.Convert(memberExpression, typeof(object)), parameter).Compile();
#else
                    getter = obj => info.GetValue(obj);
#endif
                    GetterDict.Add(info, getter);
                }
                if (!SetterDict.TryGetValue(info, out var setter))
                {
#if false
                    var targetParameter = Expression.Parameter(typeof(object), "target");
                    var valueParameter = Expression.Parameter(typeof(object), "value");
                    
                    var memberExpression = member switch
                    {
                        FieldInfo    info => Expression.Field   (Expression.Convert(targetParameter, info.DeclaringType), info),
                        PropertyInfo info => Expression.Property(Expression.Convert(targetParameter, info.DeclaringType), info),
                        _ => null
                    };
                    var assignExpression = Expression.Assign(memberExpression, Expression.Convert(valueParameter, valueType));
                    var lambda = Expression.Lambda<Action<object, object>>(assignExpression, targetParameter, valueParameter);
                    setter = lambda.Compile();
#else
                    setter = (obj, val) => info.SetValue(obj, val);
#endif
                    SetterDict.Add(info, setter);
                }
                var drawer = RuntimeDrawerFactory
                    .FromValueType(info.FieldType)
                    .Label(ProcessName(info.Name))
                    .AddAttribute(info)
                    .Build();
                AddDrawer(drawer, () => getter(value), val => setter(value, val));
            }
        }
        public abstract bool FieldFilter(FieldInfo info);
        public static string ProcessName(string name)
        {
            if (name.Length < 1) return name;
            StringBuilder sb = new();
            sb.Append(char.ToUpperInvariant(name[0]));
            bool isUpper = true;
            for (int i = 1; i < name.Length; i++)
            {
                if (!char.IsLetter(name[i]))
                {
                    sb.Append(name[i]);
                    continue;
                }
                if (char.IsUpper(name[i]))
                {
                    if (!isUpper)
                        sb.Append(' ');
                    sb.Append(name[i]);
                    isUpper = true;
                }
                else
                {
                    sb.Append(name[i]);
                    isUpper = false;
                }
            }
            return sb.ToString();
        }
    }
}