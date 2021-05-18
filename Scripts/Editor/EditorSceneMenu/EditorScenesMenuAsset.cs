using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Game.Ed.ScenesMenu
{
    public class EditorScenesMenuAsset : ScriptableObject
    {
        public List<SceneAsset> scenes;
    }

    public static class EditorScenesMenuData
    {
        public static EditorScenesMenuAsset Asset { get; private set; }

        public static void Load()
        {
			if (Asset != null) return;

			string[] guids = AssetDatabase.FindAssets("t:EditorScenesMenuAsset");
			string fn = (guids.Length > 0 ? AssetDatabase.GUIDToAssetPath(guids[0]) : GetPackageFolder() + "EditorScenesMenuAsset.asset");
			Asset = AssetDatabase.LoadAssetAtPath<EditorScenesMenuAsset>(fn);
			if (Asset == null)
			{
				Asset = ScriptableObject.CreateInstance<EditorScenesMenuAsset>();
				AssetDatabase.CreateAsset(Asset, fn);
				AssetDatabase.SaveAssets();
			}
		}

		private static string GetPackageFolder()
		{
			try
			{
				string[] res = System.IO.Directory.GetFiles(Application.dataPath, "EditorScenesMenuAsset.cs", System.IO.SearchOption.AllDirectories);
				if (res.Length > 0) return "Assets" + res[0].Replace(Application.dataPath, "").Replace("EditorScenesMenuAsset.cs", "").Replace("\\", "/");
			}
			catch (System.Exception ex)
			{
				Debug.LogException(ex);
			}
			return "Assets/";
		}

		// ============================================================================================================
	}
}