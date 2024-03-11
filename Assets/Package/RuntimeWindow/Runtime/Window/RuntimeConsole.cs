using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.RuntimeWindowUtils
{
    public class RuntimeConsole : DSRuntimeWindow
    {
        public RuntimeConsole()
        {
            contentContainer.style.flexGrow = 1;
            RuntimeCommandField commandField = new RuntimeCommandField()
            {
                style =
            {
                marginTop = StyleKeyword.Auto,
                    marginBottom = 0
            }
            };
            Add(commandField);
        }
    }
}
