#if UNITY_ANDROID && !UNITY_EDITOR && !UNITY_STANDALONE_WIN && !UNITY_STANDALONE_MAC
#define ON_ANDROID
#endif

#if !ON_ANDROID
using UnityEngine;
using UnityEditor;

public class ChangeModelImportSettings : ScriptableObject
{
    [MenuItem ("Batch Edit/Model/Set Scale Factor/1.0")]
    static void SetScaleFactor_1()
	{
        SetMeshScaling(1.0f);
    }
	
	[MenuItem ("Batch Edit/Model/Set Scale Factor/0.1")]
    static void SetScaleFactor_01()
	{
        SetMeshScaling(0.1f);
    }
	
	[MenuItem ("Batch Edit/Model/Set Scale Factor/0.01")]
    static void SetScaleFactor_001()
	{
        SetMeshScaling(0.01f);
    }
	
	static void SetMeshScaling(float scale)
	{
		Object[] meshes = GetSelectedMeshes();
		//Selection.objects = new Object[0];
		foreach(Object mesh in meshes)
		{
			string path = AssetDatabase.GetAssetPath(mesh);
			ModelImporter modelImporter = AssetImporter.GetAtPath(path) as ModelImporter;
			modelImporter.globalScale = scale;
			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
		}
	}
	
	static Object[] GetSelectedMeshes()
	{
		return Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
	}
}
#endif