using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NaiveAPI.RuntimeWindowUtils
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class RuntimeCommandAttribute : Attribute
    {
        public string Name;
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public abstract class RuntimeCommandSymbolAttribute : Attribute
    {
        public const int defaultPriority = -1;
        public abstract Type TargetType { get; }
        public abstract string[] DefaultChoices { get; }
        public virtual int Priority { get => defaultPriority; }

        public string ParameterName;
        public string[] Choices;
        public bool IncludeDefaultChoice = true;

        public IEnumerable<string> VisitChoices()
        {
            if (IncludeDefaultChoice && DefaultChoices != null)
            {
                for (int i = 0;i < DefaultChoices.Length; i++)
                {
                    yield return DefaultChoices[i];
                }
            }

            if (Choices != null)
            {
                for (int i = 0; i < Choices.Length; i++)
                {
                    yield return Choices[i];
                }
            }
        } 

        public abstract bool OnProcessValue(string value, out object obj);
    }

    public class BoolCommandSymbolAttribute : RuntimeCommandSymbolAttribute
    {
        public override Type TargetType => typeof(bool);

        private string[] defaultChoices = new string[] { "true", "false" };
        public override string[] DefaultChoices => defaultChoices;

        public override bool OnProcessValue(string value, out object obj)
        {
            if (bool.TryParse(value, out bool v))
            {
                obj = v;
                return true;
            }

            obj = null;
            return false;
        }
    }

    public class IntCommandSymbolAttribute : RuntimeCommandSymbolAttribute
    {
        public override Type TargetType => typeof(int);

        public override string[] DefaultChoices => null;

        public override bool OnProcessValue(string value, out object obj)
        {
            if (int.TryParse(value, out int v))
            {
                obj = v;
                return true;
            }

            obj = null;
            return false;
        }
    }

    public class FloatCommandSymbolAttribute : RuntimeCommandSymbolAttribute
    {
        public override Type TargetType => typeof(float);

        public override string[] DefaultChoices => null;

        public override bool OnProcessValue(string value, out object obj)
        {
            if (float.TryParse(value, out float v))
            {
                obj = v;
                return true;
            }

            obj = null;
            return false;
        }
    }

    public class StringCommandSymbolAttribute : RuntimeCommandSymbolAttribute
    {
        public override Type TargetType => typeof(string);

        public override string[] DefaultChoices => null;

        public override bool OnProcessValue(string value, out object obj)
        {
            obj = value;
            return true;
        }
    }

    public class Vector2CommandSymbolAttribute : RuntimeCommandSymbolAttribute
    {
        public override Type TargetType => typeof(Vector2);

        private string[] defaultChoices = new string[] { "(0, 0)", "(0, 1)", "(1, 0)", "(1, 0)", "(1, 0)", "(1, 0)", "(1, 0)" };
        public override string[] DefaultChoices => defaultChoices;

        public override bool OnProcessValue(string value, out object obj)
        {
            obj = null;
            string[] strVector = value.Replace("(", "").Replace(")", "").Split(",", StringSplitOptions.None);

            if (strVector.Length != 2) return false;

            if (float.TryParse(strVector[0], out float f1) &&
                float.TryParse(strVector[1], out float f2))
            {
                obj = new Vector2(f1, f2);
                return true;
            }

            return false;
        }
    }
}
