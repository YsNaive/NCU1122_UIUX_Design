using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;


namespace NaiveAPI.RuntimeWindowUtils
{
    public class RuntimeCommandInfo
    {
        public MethodInfo Info;
        public RuntimeCommandAttribute Attribute;
        public List<RuntimeCommandSymbolAttribute> ParamTypeAttributes;

        public RuntimeCommandInfo(MethodInfo info, RuntimeCommandAttribute attribute)
        {
            Info = info;
            Attribute = attribute;
            ParamTypeAttributes = new List<RuntimeCommandSymbolAttribute>();
        }

        public RuntimeCommandSymbolAttribute GetSymbolAttributes(string paramName)
        {
            for (int i = 0; i < ParamTypeAttributes.Count; i++)
            {
                if (ParamTypeAttributes[i].ParameterName == paramName)
                {
                    return ParamTypeAttributes[i];
                }
            }

            return null;
        }

        public RuntimeCommandSymbolAttribute GetCommandTypeAttribute(string paramName)
        {
            foreach (var typeAttributes in ParamTypeAttributes)
            {
                if (typeAttributes.ParameterName == paramName)
                {
                    return typeAttributes;
                }
            }

            return null;
        }

        public IEnumerable<string> GetCommandInfoChoice(int paramIndex)
        {
            ParameterInfo[] parameters = Info.GetParameters();
            if (parameters.Length <= paramIndex) return Enumerable.Empty<string>();
            string paramName = parameters[paramIndex].Name;

            RuntimeCommandSymbolAttribute paramTypeAttribute = GetCommandTypeAttribute(paramName);
            if (paramTypeAttribute != null)
            {
                return paramTypeAttribute.VisitChoices();
            }

            ParameterInfo param = parameters[paramIndex];
            return RuntimeCommandHandler.GetDefaultChoice(param.ParameterType);
        }

        public bool ToObject(string paramName, Type paramType, string paramValue, out object obj)
        {
            obj = null;
            if (paramType.IsSubclassOf(typeof(Enum)))
            {
                int index = Array.IndexOf(Enum.GetNames(paramType), paramValue);
                if (index == -1) return false;
                obj = Enum.GetValues(paramType).GetValue(index);
                return true;
            }

            RuntimeCommandSymbolAttribute attribute = GetSymbolAttributes(paramName);

            if (attribute != null)
            {
                return attribute.OnProcessValue(paramValue, out obj);
            }

            if (RuntimeCommandHandler.DefaultSymbols.ContainsKey(paramType))
            {
                return RuntimeCommandHandler.DefaultSymbols[paramType].OnProcessValue(paramValue, out obj);
            }

            return false;
        }
    }
}
