using UnityEngine;
using UnityEditor;

namespace Game.Ed.ScenesMenu
{
    public class EditorScenesMenuWindow: EditorWindow
    {
        public static void Open()
        {
            var window = GetWindow<EditorScenesMenuWindow>(true, "ScenesMenu Setup");
            window.minSize = new Vector2(300, 300);
            window.Show();
        }

        private SerializedObject asset;
        private SerializedProperty prop;

        private void OnEnable()
        {
            asset = new SerializedObject(EditorScenesMenuData.Asset);
            prop = asset.FindProperty("scenes");
        }

        private void OnGUI()
		{
            EditorGUILayout.PropertyField(prop);
            asset.ApplyModifiedProperties();
        }

        private void OnLostFocus()
        {
            EditorScenesMenuDropdown.UpdateMenu();
        }

		private void OnDestroy()
		{
            EditorScenesMenuDropdown.UpdateMenu();
        }

		// ============================================================================================================
	}
}