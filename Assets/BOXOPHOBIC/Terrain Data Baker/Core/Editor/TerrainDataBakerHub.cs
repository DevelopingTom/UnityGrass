// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using Boxophobic.StyledGUI;
using Boxophobic.Utils;

public class TerrainDataBakerHub : EditorWindow
{
    string assetFolder = "Assets/BOXOPHOBIC/Terrain Data Baker";

    int assetVersion;
    string bannerVersion;

    Color bannerColor;
    string bannerText;
    static TerrainDataBakerHub window;

    [MenuItem("Window/BOXOPHOBIC/Terrain Data Baker/Hub", false, 1036)]
    public static void ShowWindow()
    {
        window = GetWindow<TerrainDataBakerHub>(false, "Terrain Data Baker Hub", true);
        window.minSize = new Vector2(300, 200);
    }

    void OnEnable()
    {
        //Safer search, there might be many user folders
        string[] searchFolders;

        searchFolders = AssetDatabase.FindAssets("Terrain Data Baker");

        for (int i = 0; i < searchFolders.Length; i++)
        {
            if (AssetDatabase.GUIDToAssetPath(searchFolders[i]).EndsWith("Terrain Data Baker.pdf"))
            {
                assetFolder = AssetDatabase.GUIDToAssetPath(searchFolders[i]);
                assetFolder = assetFolder.Replace("/Terrain Data Baker.pdf", "");
            }
        }

        assetVersion = SettingsUtils.LoadSettingsData(assetFolder + "/Core/Editor/Version.asset", -99);

        bannerVersion = assetVersion.ToString();
        bannerVersion = bannerVersion.Insert(1, ".");
        bannerVersion = bannerVersion.Insert(3, ".");

        bannerColor = new Color(0.62f, 0.71f, 0.20f);
        bannerText = "Terrain Data Baker";
    }

    void OnGUI()
    {
        DrawToolbar();

        StyledGUI.DrawWindowBanner(bannerColor, bannerText, bannerVersion);

        GUILayout.BeginHorizontal();
        GUILayout.Space(15);

        EditorGUILayout.HelpBox("The included tool is compatible by deafult with any render pipeline!", MessageType.Info, true);

        GUILayout.Space(13);
        GUILayout.EndHorizontal();
    }

    void DrawToolbar()
    {
        var GUI_TOOLBAR_EDITOR_WIDTH = this.position.width / 4.0f + 1;

        var styledToolbar = new GUIStyle(EditorStyles.toolbarButton)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Normal,
            fontSize = 11,
        };

        GUILayout.Space(1);
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Discord Server", styledToolbar, GUILayout.Width(GUI_TOOLBAR_EDITOR_WIDTH)))
        {
            Application.OpenURL("https://discord.com/invite/znxuXET");
        }
        GUILayout.Space(-1);

        if (GUILayout.Button("Documentation", styledToolbar, GUILayout.Width(GUI_TOOLBAR_EDITOR_WIDTH)))
        {
            Application.OpenURL("https://docs.google.com/document/d/1przDXu3yQmWUDIkTk5_J0dhGL0oOoaI2Gs6yv3Eh2HM/edit#");
        }
        GUILayout.Space(-1);

        if (GUILayout.Button("Changelog", styledToolbar, GUILayout.Width(GUI_TOOLBAR_EDITOR_WIDTH)))
        {
            Application.OpenURL("https://docs.google.com/document/d/1przDXu3yQmWUDIkTk5_J0dhGL0oOoaI2Gs6yv3Eh2HM/edit#heading=h.1rbujejuzjce");
        }
        GUILayout.Space(-1);

        if (GUILayout.Button("Write A Review", styledToolbar, GUILayout.Width(GUI_TOOLBAR_EDITOR_WIDTH)))
        {
            //Application.OpenURL("");
        }
        GUILayout.Space(-1);

        GUILayout.EndHorizontal();
    }
}


