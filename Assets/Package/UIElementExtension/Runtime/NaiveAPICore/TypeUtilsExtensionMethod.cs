using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace NaiveAPI.UITK
{
    public static class TypeUtilsExtensionMethod
    {
        public static Type FieldOrPropertyType(this MemberInfo info)
        {
            return info switch
            {
                FieldInfo f => f.FieldType,
                PropertyInfo p => p.PropertyType,
                _ => null
            };
        }
        public static bool IsStatic(this MemberInfo info)
        {
            return info switch
            {
               MethodBase   ret => ret.IsStatic,
               FieldInfo    ret => ret.IsStatic,
               PropertyInfo ret => ret.GetAccessors()[0].IsStatic,
               _ => throw new NotSupportedException("This method only work with Method/Field/Property")
            };
        }
    }
}
