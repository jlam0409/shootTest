#if UNITY_ANDROID && !UNITY_EDITOR && !UNITY_STANDALONE_WIN && !UNITY_STANDALONE_MAC
#define ON_ANDROID
#endif

#if !ON_ANDROID
using UnityEngine;
using UnityEditor;

// /////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Batch texture import settings modifier.
//
// Modifies all selected textures in the project window and applies the requested modification on the
// textures. If GUI is selected, the type will be set to "GUI" and format to "Truecolour". If Object is
// selected the type will be set to "Image" and the format to "Compressed". 
//
// November 2011. Based on jj's audio and model import settings batch modifier.
// November 2011. Added - choosing GUI will set the texture to not scale if texture size is not power of two.
//
// /////////////////////////////////////////////////////////////////////////////////////////////////////////


public class ChangeTextureImportSettings : ScriptableObject {

    [MenuItem ("Batch Edit/Texture/Texture Type/GUI")]
    static void SetTextureType_Gui() {
        SelectedTextureTypeSetting(TextureImporterType.GUI);
		SelectedTextureFormatSetting(TextureImporterFormat.AutomaticTruecolor);
    }

    [MenuItem ("Batch Edit/Texture/Texture Type/Object")]
    static void SetTextureType_Object() {
        SelectedTextureTypeSetting(TextureImporterType.Image);
		SelectedTextureFormatSetting(TextureImporterFormat.AutomaticCompressed);
    }
	
	
	static void SelectedTextureTypeSetting(TextureImporterType newType) {

        Object[] textures = GetSelectedTextures();
        foreach (Texture texture in textures) {
            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            textureImporter.textureType = newType;
            AssetDatabase.ImportAsset(path);
        }
	
	}
	
	static void SelectedTextureFormatSetting(TextureImporterFormat newFormat) {

        Object[] textures = GetSelectedTextures();
        foreach (Texture texture in textures) {
            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
			textureImporter.npotScale = TextureImporterNPOTScale.None;
            textureImporter.textureFormat = newFormat;
            AssetDatabase.ImportAsset(path);
        }
	
	}
	
	
	static Object[] GetSelectedTextures()
    {
        return Selection.GetFiltered(typeof(Texture), SelectionMode.DeepAssets);
    }
	
}
#endif