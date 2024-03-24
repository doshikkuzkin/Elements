#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LevelGenerator
{
	public static class AssetDataBaseTool
	{
#if UNITY_EDITOR
		public static void AssertHasFolder(string parentFolder, string folderName)
		{
			if (!AssetDatabase.IsValidFolder($"{parentFolder}/{folderName}"))
			{
				AssetDatabase.CreateFolder(parentFolder, folderName);
				AssetDatabase.Refresh();
			}
		}

		public static void CreateAsset(UnityEngine.Object asset, string path)
		{
			AssetDatabase.CreateAsset(asset, path);
		}
		
		public static T LoadAssetAtPath<T> (string path) where T : UnityEngine.Object
		{
			return AssetDatabase.LoadAssetAtPath<T>(path);
		}

		public static void Save()
		{
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
#endif
	}
}