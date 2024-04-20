using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    [Serializable]
    public class DefaultDrawerSettings
    {
        public bool AffectOnDerived = true;
        public Sprite Icon = null;
        public List<string> HideMember = new();
        public List<string> ExposeMember = new();

        public bool IsDefault()
        {
            return (HideMember.Count == 0)   &&
                   (ExposeMember.Count == 0) &&
                   (Icon == null)            &&
                   (AffectOnDerived = true);
        }
    }

    [CustomRuntimeDrawer(typeof(object), Priority = -99, DrawAssignableType = true)]
    public class DefaultDrawer : StandardDrawer<object>
    {
        private static Dictionary<Type, MemberInfo[]> ExposeMemberTable = new();
        private static Dictionary<Type, Sprite> IconTable = new();
        private static Dictionary<MemberInfo, Func<object, object>> GetterTable = new();
        private static Dictionary<MemberInfo, Action<object, object>> SetterTable = new();
        public static Func<object, object> GetGetter(MemberInfo member)
        {
            if (GetterTable.TryGetValue(member, out var getter))
                return getter;

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
            getter = member switch
            {
                FieldInfo info => obj => info.GetValue(obj),
                PropertyInfo info => obj => info.GetValue(obj),
                _ => throw new NotSupportedException("Can't handle this Member with DefaultDrawer")
            };
#endif
            GetterTable.Add(member, getter);
            return getter;
        }
        public static Action<object, object> GetSetter(MemberInfo member)
        {
            if (SetterTable.TryGetValue(member, out var setter))
                return setter;
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
            setter = member switch
            {
                FieldInfo info => (obj, val) => info.SetValue(obj, val),
                PropertyInfo info => (obj, val) => info.SetValue(obj, val),
                _ => throw new NotSupportedException("Can't handle this Member with DefaultDrawer")
            };
#endif
            SetterTable.Add(member, setter);
            return setter;
        }
        public static IEnumerable<MemberInfo> GetExposeableMember(Type type)
        {
            return type
                .GetMembers(BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(member =>
                {
                    if (member.Name[0] == '<')
                        return false;
                    return member switch
                    {
                        FieldInfo => true,
                        PropertyInfo info => (info.CanRead && info.CanWrite),
                        _ => false
                    };
                });
        }
        private static void ValidTypeInfo(Type type)
        {
            HashSet<string> hideSet = new();
            HashSet<string> exposeSet = new();
            Sprite icon = null;
            foreach (var settings in UIElementExtensionResource.Get.DefaultDrawerSettings
                .Where(pair => pair.Key == type || (pair.Value.AffectOnDerived ? type.IsSubclassOf(pair.Key) : false))
                .OrderBy(pair =>
                {
                    int deep = 0;
                    Type curr = pair.Key;
                    while (curr.BaseType != null)
                    {
                        curr = curr.BaseType;
                        deep++;
                    }
                    return deep;
                }))
            {
                foreach (var hide in settings.Value.HideMember)
                    hideSet.Add(hide);
                foreach (var expose in settings.Value.ExposeMember)
                    exposeSet.Add(expose);
                if(settings.Value.Icon != null)
                    icon = settings.Value.Icon;
            }
            IconTable.Add(type, icon);
            var members = GetExposeableMember(type)
                .Where(member =>
                {
                    if (DefaultMemberFilter(member))
                        return !hideSet.Contains(member.Name);
                    else
                        return exposeSet.Contains(member.Name);
                }).ToArray();

            ExposeMemberTable.Add(type, members);
        }
        public static IEnumerable<MemberInfo> GetExposeMember(Type type)
        {
            if(ExposeMemberTable.TryGetValue(type, out var its))
                return its;
            ValidTypeInfo(type);
            return ExposeMemberTable[type];
        }
        public static Sprite GetDefaultIcon(Type type)
        {
            if (type == null)
                return null;
            if (IconTable.TryGetValue(type, out var icon))
                return icon;
            ValidTypeInfo(type);
            return IconTable[type];
        }
        public override bool DynamicLayout => true;

        public DefaultDrawer()
        {
            iconElement2 = new VisualElement()
            {
                style =
                    {
                        height = RSTheme.Current.LineHeight,
                        width = RSTheme.Current.LineHeight,
                    }
            };
            labelElement.Insert(labelElement.IndexOf(iconElement) + 1, iconElement2);
        }
        protected override void CreateGUI()
        {
            ClearDrawer();
            if (value == null)
                return;
            foreach (var member in GetExposeMember(value.GetType()))
            {
                var getter = GetGetter(member);
                var setter = GetSetter(member);
                var drawer = RuntimeDrawerFactory
                    .FromValueType(member.FieldOrPropertyType())
                    .Label(ProcessName(member.Name))
                    .AddAttribute(member)
                    .Build();
                AddDrawer(drawer, () => getter(value), val => setter(value, val));
                drawer.SetValue(getter(value));
            }
            SetIcon(GetDefaultIcon(value.GetType()));
        }
        public static bool DefaultMemberFilter(MemberInfo info)
        {
            Type valueType;
            bool isPublic;
            switch (info)
            {
                case FieldInfo fieldInfo:
                    valueType = fieldInfo.FieldType;
                    isPublic = fieldInfo.IsPublic;
                    break;
                default: 
                    return false;
            }
            if (info.Name.StartsWith("m_") || info.Name.StartsWith("s_"))
                return false;

            var isUnitySerialize = isPublic ? !info.IsDefined(typeof(HideInInspector), false) : info.IsDefined(typeof(SerializeField), false);
            return isUnitySerialize;
        }
        public static string ProcessName(string name)
        {
            if (name.Length < 1) return name;
            int i = 0;
            if (name.StartsWith("m_", StringComparison.Ordinal) || name.StartsWith("s_", StringComparison.Ordinal))
                i += 2;
            StringBuilder sb = new();
            sb.Append(char.ToUpperInvariant(name[i]));
            bool isUpper = true;
            for (i++ ; i < name.Length; i++)
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