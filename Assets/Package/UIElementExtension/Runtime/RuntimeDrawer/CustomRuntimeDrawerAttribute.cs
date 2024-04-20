using System;

namespace NaiveAPI.UITK
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class CustomRuntimeDrawerAttribute : Attribute
    {
        public CustomRuntimeDrawerAttribute(Type targetType)
        {
            TargetType = targetType;
        }
        public readonly Type TargetType;
        public Type RequiredAttribute = null;
        public int Priority = 0;
        public bool DrawDerivedType = false;
        public bool DrawAssignableType = false;
        public bool CanDrawType(Type type, Type attribute = null)
        {
            if(attribute != RequiredAttribute)
                return false;
            if(type == TargetType) 
                return true;
            if (DrawDerivedType)
            {
                if(type.IsSubclassOf(TargetType))
                    return true;
            }
            if (DrawAssignableType)
            {
                if(TargetType.IsAssignableFrom(type))
                    return true;
            }
            return false;
        }
    }
}