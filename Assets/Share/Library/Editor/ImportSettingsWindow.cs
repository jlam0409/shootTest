using UnityEngine;
using UnityEditor;

public class ImportSettingsWindow : EditorWindow
{
    public static bool bCreateMipmaps = false;
    public static bool bGrayScaleToAlpha = false;
	public static bool bReadable = false;
    public static bool bUseAdvancedImportSettings = false;
    public static TextureImporterFormat textureFormat = TextureImporterFormat.AutomaticTruecolor;
    public static MaxTextureSize maxTextureSize = MaxTextureSize.x1024;
    public static TextureImporterNPOTScale npotScale = TextureImporterNPOTScale.None;
    public static TextureWrapMode wrap = TextureWrapMode.Clamp;
    public static bool bCompressTexturesOnImport = false;
    private static GUIStyle guiStyle;

    public enum MaxTextureSize
    {
        x32 = 32,
        x64 = 64,
        x128 = 128,
        x256 = 256,
        x512 = 512,
        x1024 = 1024,
        x2048 = 2048,
        x4096 = 4096
    }

    [MenuItem("Window/Import Settings")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        ImportSettingsWindow window = (ImportSettingsWindow)EditorWindow.GetWindow(typeof(ImportSettingsWindow));
        window.title = "ImportSettings";
        window.Show();
    }

    void OnGUI()
    {
        if (guiStyle == null)
        {
            guiStyle = new GUIStyle(EditorStyles.label);
            guiStyle.alignment = TextAnchor.UpperLeft;
            guiStyle.normal.textColor = Color.red; // <img src="./images/smilies/1.gif" alt=":)" title="Happy" />
            guiStyle.font = EditorStyles.boldFont;
        }
        EditorGUILayout.BeginVertical();
        if (!bUseAdvancedImportSettings)
            GUILayout.Label("Using Default Import Settings...", "BoldLabel");
        else
            GUILayout.Label("Overriding Default Import Settings!", guiStyle);


        if (!bCompressTexturesOnImport)
            GUILayout.Label("Compressing Only on build...", "BoldLabel");
        else
            GUILayout.Label("Compressing on import!", guiStyle);

        bUseAdvancedImportSettings =
                EditorGUILayout.BeginToggleGroup("Override default settings with those\nset here", bUseAdvancedImportSettings);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Create mip maps:\t\t\t");
        bCreateMipmaps = EditorGUILayout.Toggle(bCreateMipmaps);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("GrayScale to Alpha:\t\t");
        bGrayScaleToAlpha = EditorGUILayout.Toggle(bGrayScaleToAlpha);
        EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Readable:\t\t\t\t\t");
        bReadable = EditorGUILayout.Toggle(bReadable);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Wrap Mode:\t\t\t\t\t");
        wrap = (TextureWrapMode)EditorGUILayout.EnumPopup(wrap);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Texture format:\t\t\t");
        textureFormat = (TextureImporterFormat)EditorGUILayout.EnumPopup((System.Enum)textureFormat);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Max Texture Size:\t\t\t");
        maxTextureSize = (MaxTextureSize)EditorGUILayout.EnumPopup(maxTextureSize);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Non-power-of-2 scale:\t");
        npotScale = (TextureImporterNPOTScale)EditorGUILayout.EnumPopup(npotScale);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndToggleGroup();

        EditorGUILayout.Separator();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Compress textures on import:\t\t\t");
        bCompressTexturesOnImport = EditorGUILayout.Toggle(bCompressTexturesOnImport);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }
}

public class CustomTextureImporter : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        if (!ImportSettingsWindow.bUseAdvancedImportSettings)
            return;

        TextureImporter textureImporter = (TextureImporter)assetImporter;
        textureImporter.mipmapEnabled = ImportSettingsWindow.bCreateMipmaps;
        textureImporter.textureFormat = ImportSettingsWindow.textureFormat;
        textureImporter.maxTextureSize = (int)ImportSettingsWindow.maxTextureSize;
        textureImporter.npotScale = ImportSettingsWindow.npotScale;
        textureImporter.grayscaleToAlpha = ImportSettingsWindow.bGrayScaleToAlpha;
		textureImporter.isReadable = ImportSettingsWindow.bReadable;
    }

    void OnPostprocessTexture(Texture2D texture)
    {
        if (!ImportSettingsWindow.bUseAdvancedImportSettings)
            return;
        texture.wrapMode = ImportSettingsWindow.wrap;
        if (ImportSettingsWindow.bCompressTexturesOnImport)
            texture.Compress(true);
    }
	
	void OnPreprocessAudio()
	{
        if (!ImportSettingsWindow.bUseAdvancedImportSettings)
            return;
        AudioImporter audioImporter = (AudioImporter)assetImporter;
		audioImporter.threeD = false;
		
	}
	
	void OnPostprocessAudio(AudioClip audio)
	{
        if (!ImportSettingsWindow.bUseAdvancedImportSettings)
            return;
		AudioImporter audioImporter = (AudioImporter)assetImporter;
		audioImporter.format = AudioImporterFormat.Compressed;
	}
}