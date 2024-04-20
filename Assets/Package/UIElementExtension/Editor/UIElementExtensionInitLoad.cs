using UnityEditor;

namespace NaiveAPI_Editor.UITK
{
    [InitializeOnLoad]
    public class UIElementExtensionInitLoad
    {
        public const string Symbol = "NAIVEAPI_UIELEMENT_EXTENSION";
        static UIElementExtensionInitLoad()
        {
            var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (!symbols.Contains(Symbol))
            {
                symbols += ";" + Symbol;
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
            }
        }
    }
}
