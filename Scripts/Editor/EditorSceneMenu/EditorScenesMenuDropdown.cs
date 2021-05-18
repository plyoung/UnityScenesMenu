using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Game.Ed.ScenesMenu
{
	[InitializeOnLoad]
	public static class EditorScenesMenuDropdown
	{
		private static ScriptableObject toolbar;
		private static ToolbarMenu toolbarSceneMenu;
		private static Type toolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");

		// ------------------------------------------------------------------------------------------------------------------

		static EditorScenesMenuDropdown()
		{
			EditorScenesMenuData.Load();
			EditorApplication.update -= OnUpdate;
			EditorApplication.update += OnUpdate;
		}

		public static void UpdateMenu()
		{
			if (toolbarSceneMenu == null) return;

			toolbarSceneMenu.menu.MenuItems().Clear();

			if (EditorScenesMenuData.Asset != null && EditorScenesMenuData.Asset.scenes?.Count > 0)
			{
				foreach (var s in EditorScenesMenuData.Asset.scenes)
				{
					if (s == null) continue;
					var nm = AssetDatabase.GetAssetPath(s).Substring(7).Replace("/", "\\");
					toolbarSceneMenu.menu.AppendAction(nm, a => OpenScene(s), a => DropdownMenuAction.Status.Normal);
				}
			}

			toolbarSceneMenu.menu.AppendSeparator();
			toolbarSceneMenu.menu.AppendAction("Settings", a => EditorScenesMenuWindow.Open(), a => DropdownMenuAction.Status.Normal);
		}

		private static void OpenScene(SceneAsset sceneAsset)
		{
			EditorSceneManager.SaveOpenScenes();
			EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(sceneAsset));
		}

		private static void OnUpdate()
		{
			if (toolbar == null)
			{
				var toolbars = Resources.FindObjectsOfTypeAll(toolbarType);
				toolbar = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;
				if (toolbar == null) return;

				// some reflection to get to the visual tree of the toolbar
				var guiViewType = typeof(Editor).Assembly.GetType("UnityEditor.GUIView");
				var iWindowBackendType = typeof(Editor).Assembly.GetType("UnityEditor.IWindowBackend");
				var guiView_windowBackend = guiViewType.GetProperty("windowBackend", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				var windowBackend_viewVisualTree = iWindowBackendType.GetProperty("visualTree", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				var windowBackend = guiView_windowBackend.GetValue(toolbar);
				var visualTree = (VisualElement)windowBackend_viewVisualTree.GetValue(windowBackend, null);

				// create the scenes dropdown
				toolbarSceneMenu = new ToolbarMenu { text = "Scenes" };
				UpdateMenu();

				// set the styles
				var toolbarLayersDropdown = visualTree.Q("LayersDropdown");
				CopyStyleClasses(toolbarLayersDropdown, toolbarSceneMenu);
				toolbarSceneMenu.style.minWidth = 80;
				toolbarSceneMenu.style.maxWidth = 200;

				// add the new menu to toolbar
				visualTree.Q("ToolbarZoneLeftAlign").Add(toolbarSceneMenu);

				// done
				EditorApplication.update -= OnUpdate;
			}
		}

		private static void CopyStyleClasses(VisualElement src, VisualElement dst)
		{
			dst.ClearClassList();
			foreach (var s in src.GetClasses()) dst.AddToClassList(s);

			var srcChildren = src.Children().ToList();
			var dstChildren = dst.Children().ToList();
			if (srcChildren.Count == dstChildren.Count)
			{
				for (int i = 0; i < srcChildren.Count; i++)
				{
					CopyStyleClasses(srcChildren[i], dstChildren[i]);
				}
			}
			else
			{
				Debug.LogWarning("This method should only be used on elements that are similar.");
			}
		}

		// ============================================================================================================
	}
}