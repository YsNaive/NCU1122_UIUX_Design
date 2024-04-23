using NaiveAPI.UITK;
using UnityEditor;
using UnityEngine;

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

            if(Resources.Load<UIElementExtensionResource>(UIElementExtensionResource.SourceFileName) == null)
            {
                var path = AssetDatabase.GUIDToAssetPath(UIElementExtensionResource.ResourceFolderGUID) + "\\" + UIElementExtensionResource.SourceFileName + ".asset";
                AssetDatabase.CreateAsset(UIElementExtensionResource.Get, path);
                AssetDatabase.Refresh();
                Debug.Log($"UIElementExtensionResource Asset Not Found. Auto Create at: {path}");
            }
        }
    }
}
