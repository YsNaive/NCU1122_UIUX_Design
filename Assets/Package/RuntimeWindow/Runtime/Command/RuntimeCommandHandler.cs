using NaiveAPI.DocumentBuilder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace NaiveAPI.RuntimeWindowUtils
{
    public static class RuntimeCommandHandler
    {
        private static List<RuntimeCommandInfo> activeMethods;
        public static Dictionary<Type, RuntimeCommandSymbolAttribute> DefaultSymbols;
        public static object Caller;
        public const int BuildInDefaultPriority = -1;
        public static object ObjValue = null;

        public static IEnumerable<RuntimeCommandInfo> ActiveMethods => activeMethods;

        static RuntimeCommandHandler()
        {
            activeMethods = new List<RuntimeCommandInfo>();

            foreach (Type type in TypeReader.ActiveTypes)
            {
                var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                                           .Where((method) => method.IsDefined(typeof(RuntimeCommandAttribute)));
                foreach (var method in methods)
                {
                    RuntimeCommandInfo commandInfo = new RuntimeCommandInfo(method, method.GetCustomAttribute<RuntimeCommandAttribute>());
                    if (method.IsDefined(typeof(RuntimeCommandSymbolAttribute)))
                    {
                        foreach (var typeAttribute in method.GetCustomAttributes<RuntimeCommandSymbolAttribute>())
                        {
                            commandInfo.ParamTypeAttributes.Add(typeAttribute);
                        }
                    }

                    activeMethods.Add(commandInfo);
                }
            }

            Debug.Log(activeMethods.Count);

            DefaultSymbols = new Dictionary<Type, RuntimeCommandSymbolAttribute>();

            foreach (Type type in TypeReader.ActiveTypes
                                            .Where((t) => !t.IsAbstract && t.IsSubclassOf(typeof(RuntimeCommandSymbolAttribute))))
            {
                var attribute = (RuntimeCommandSymbolAttribute)Activator.CreateInstance(type);

                if (DefaultSymbols.ContainsKey(attribute.TargetType))
                {
                    if (attribute.Priority > DefaultSymbols[attribute.TargetType].Priority)
                        DefaultSymbols[attribute.TargetType] = attribute;
                }
                else
                {
                    DefaultSymbols.Add(attribute.TargetType, attribute);
                }
            }
        }

        public static RuntimeCommandInfo GetCommandInfo(int index)
        {
            return activeMethods[index];
        }

        public static RuntimeCommandInfo GetCommandInfo(string name)
        {
            foreach (RuntimeCommandInfo info in activeMethods)
            {
                string methodName = info.Attribute.Name ?? info.Info.Name;

                if (name == methodName)
                {
                    return info;
                }
            }

            return null;
        }

        public static IEnumerable<string> GetDefaultChoice(Type type)
        {
            if (type.IsSubclassOf(typeof(Enum)))
            {
                return Enum.GetNames(type);
            }

            if (DefaultSymbols.ContainsKey(type))
            {
                return DefaultSymbols[type].VisitChoices();
            }

            return Enumerable.Empty<string>();
        }
    }
}
