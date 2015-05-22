#if UNITY_ANDROID && !UNITY_EDITOR && !UNITY_STANDALONE_WIN && !UNITY_STANDALONE_MAC
#define ON_ANDROID
#endif

#if !ON_ANDROID
using UnityEngine;
using UnityEditor;

// /////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Batch audio import settings modifier.
//
// Modifies all selected audio clips in the project window and applies the requested modification on the
// audio clips. Idea was to have the same choices for multiple files as you would have if you open the
// import settings of a single audio clip. Put this into Assets/Editor and once compiled by Unity you find
// the new functionality in Custom -> Sound. Enjoy! :-)
//
// April 2010. Based on Martin Schultz's texture import settings batch modifier.
//
// /////////////////////////////////////////////////////////////////////////////////////////////////////////
public class ChangeAudioImportSettings : ScriptableObject {

    [MenuItem ("Batch Edit/Sound/Toggle audio compression/Disable")]
    static void ToggleCompression_Disable() {
        SelectedToggleCompressionSettings(AudioImporterFormat.Native);
    }

    [MenuItem ("Batch Edit/Sound/Toggle audio compression/Enable")]
    static void ToggleCompression_Enable() {
        SelectedToggleCompressionSettings(AudioImporterFormat.Compressed);
    }
	
	// ----------------------------------------------------------------------------
	//JJ: Added Format Support.
	
	[MenuItem ("Batch Edit/Sound/Set Compression Format/NativeWAV")]
    static void SetCompressionType_NativeWAV() {
		SelectedSetCompressionFormat(false);
    }
	
    [MenuItem ("Batch Edit/Sound/Set Compression Format/Compressed")]
    static void SetCompressionType_Compressed() {
		SelectedSetCompressionFormat(true);
    }

    // ----------------------------------------------------------------------------

    [MenuItem ("Batch Edit/Sound/Set audio compression bitrate (kbps)/32")]
    static void SetCompressionBitrate_32kbps() {
        SelectedSetCompressionBitrate(32000);
    }

    [MenuItem ("Batch Edit/Sound/Set audio compression bitrate (kbps)/64")]
    static void SetCompressionBitrate_64kbps() {
        SelectedSetCompressionBitrate(64000);
    }

    [MenuItem ("Batch Edit/Sound/Set audio compression bitrate (kbps)/96")]
    static void SetCompressionBitrate_96kbps() {
        SelectedSetCompressionBitrate(96000);
    }
	
	[MenuItem ("Batch Edit/Sound/Set audio compression bitrate (kbps)/110")]
    static void SetCompressionBitrate_110kbps() {
        SelectedSetCompressionBitrate(110000);
    }

    [MenuItem ("Batch Edit/Sound/Set audio compression bitrate (kbps)/128")]
    static void SetCompressionBitrate_128kbps() {
        SelectedSetCompressionBitrate(128000);
    }

    [MenuItem ("Batch Edit/Sound/Set audio compression bitrate (kbps)/144")]
    static void SetCompressionBitrate_144kbps() {
        SelectedSetCompressionBitrate(144000);
    }

    [MenuItem ("Batch Edit/Sound/Set audio compression bitrate (kbps)/156 (default)")]
    static void SetCompressionBitrate_156kbps() {
        SelectedSetCompressionBitrate(156000);
    }

    [MenuItem ("Batch Edit/Sound/Set audio compression bitrate (kbps)/160")]
    static void SetCompressionBitrate_160kbps() {
        SelectedSetCompressionBitrate(160000);
    }

    [MenuItem ("Batch Edit/Sound/Set audio compression bitrate (kbps)/192")]
    static void SetCompressionBitrate_192kbps() {
        SelectedSetCompressionBitrate(192000);
    }

    [MenuItem ("Batch Edit/Sound/Set audio compression bitrate (kbps)/224")]
    static void SetCompressionBitrate_224kbps() {
        SelectedSetCompressionBitrate(224000);
    }

    [MenuItem ("Batch Edit/Sound/Set audio compression bitrate (kbps)/240")]
    static void SetCompressionBitrate_240kbps() {
        SelectedSetCompressionBitrate(240000);
    }

    // ----------------------------------------------------------------------------

    [MenuItem ("Batch Edit/Sound/Load Type/Stream from disc")]
    static void ToggleDecompressOnLoad_Disable() {
        SelectedToggleDecompressOnLoadSettings(AudioImporterLoadType.StreamFromDisc);
    }

    [MenuItem ("Batch Edit/Sound/Load Type/Descompress on Load")]
    static void ToggleDecompressOnLoad_Enable() {
        SelectedToggleDecompressOnLoadSettings(AudioImporterLoadType.DecompressOnLoad);
    } 
	
	[MenuItem ("Batch Edit/Sound/Load Type/CompressedInMemory")]
    static void ToggleDecompressOnLoad_Enable2() {
        SelectedToggleDecompressOnLoadSettings(AudioImporterLoadType.CompressedInMemory);
    }

    // ----------------------------------------------------------------------------

    [MenuItem ("Batch Edit/Sound/Toggle 3D sound/Disable")]
    static void Toggle3DSound_Disable() {
        SelectedToggle3DSoundSettings(false);
    }

    [MenuItem ("Batch Edit/Sound/Toggle 3D sound/Enable")]
    static void Toggle3DSound_Enable() {
        SelectedToggle3DSoundSettings(true);
    }

    // ----------------------------------------------------------------------------

    [MenuItem ("Batch Edit/Sound/Toggle mono/Auto")]
    static void ToggleForceToMono_Auto() {
        SelectedToggleForceToMonoSettings(false);
    }

    [MenuItem ("Batch Edit/Sound/Toggle mono/Forced")]
    static void ToggleForceToMono_Forced() {
        SelectedToggleForceToMonoSettings(true);
    }

    // ----------------------------------------------------------------------------
	 [MenuItem ("Batch Edit/Sound/Hardware Decoding/Enabled")]
    static void enable_Hardware_yes() {
        enableHardwareDecoding(true);
    }
	[MenuItem ("Batch Edit/Sound/Hardware Decoding/Disabled")]
	static void enable_Hardware_no() {
        enableHardwareDecoding(false);
    }
	
	
	
	static void enableHardwareDecoding ( bool enable )
	{
		Object[] audioclips = GetSelectedAudioclips();
        //Selection.objects = new Object[0];
        foreach (AudioClip audioclip in audioclips) {
            string path = AssetDatabase.GetAssetPath(audioclip);
            AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;
            audioImporter.hardware = enable;
            AssetDatabase.ImportAsset(path);
			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
	}

    static void SelectedToggleCompressionSettings(AudioImporterFormat newFormat) {

        Object[] audioclips = GetSelectedAudioclips();
        //Selection.objects = new Object[0];
        foreach (AudioClip audioclip in audioclips) {
            string path = AssetDatabase.GetAssetPath(audioclip);
            AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;
            audioImporter.format = newFormat;
            AssetDatabase.ImportAsset(path);
			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
    }

    static void SelectedSetCompressionBitrate(float newCompressionBitrate) {

        Object[] audioclips = GetSelectedAudioclips();
        //Selection.objects = new Object[0];
        foreach (AudioClip audioclip in audioclips) {
            string path = AssetDatabase.GetAssetPath(audioclip);
            AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;
            audioImporter.compressionBitrate = (int)newCompressionBitrate;
            AssetDatabase.ImportAsset(path);
			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
    }
	
	static void SelectedSetCompressionFormat(bool compressed)
	{
        Object[] audioclips = GetSelectedAudioclips();
        //Selection.objects = new Object[0];
        foreach (AudioClip audioclip in audioclips) {
            string path = AssetDatabase.GetAssetPath(audioclip);
            AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;
			if(compressed)
			{
				audioImporter.format = AudioImporterFormat.Compressed;
			}
			else
			{
				audioImporter.format = AudioImporterFormat.Native;
			}
            AssetDatabase.ImportAsset(path);
			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
    }

    static void SelectedToggleDecompressOnLoadSettings(AudioImporterLoadType enabled) {

        Object[] audioclips = GetSelectedAudioclips();
        //Selection.objects = new Object[0];
        foreach (AudioClip audioclip in audioclips) {
            string path = AssetDatabase.GetAssetPath(audioclip);
            AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;
            audioImporter.loadType = enabled;
            AssetDatabase.ImportAsset(path);
			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
    }

    static void SelectedToggle3DSoundSettings(bool enabled) {

        Object[] audioclips = GetSelectedAudioclips();
        //Selection.objects = new Object[0];
        foreach (AudioClip audioclip in audioclips) {
            string path = AssetDatabase.GetAssetPath(audioclip);
            AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;
            audioImporter.threeD = enabled;
			AssetDatabase.ImportAsset(path);
			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
    }

    static void SelectedToggleForceToMonoSettings(bool enabled) {

        Object[] audioclips = GetSelectedAudioclips();
        //Selection.objects = new Object[0];
        foreach (AudioClip audioclip in audioclips) {
            string path = AssetDatabase.GetAssetPath(audioclip);
            AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;
            audioImporter.forceToMono = enabled;
            AssetDatabase.ImportAsset(path);
			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
    }

    static Object[] GetSelectedAudioclips()
    {
        return Selection.GetFiltered(typeof(AudioClip), SelectionMode.DeepAssets);
    }
}
#endif