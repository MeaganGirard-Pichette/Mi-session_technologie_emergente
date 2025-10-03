using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class FavoritesWindow : EditorWindow
{
    private List<string> favoriteGuids = new List<string>();
    private Vector2 scrollPos;
    private const string prefsKey = "FavoritesWindow_AssetGUIDs";

    [MenuItem("Window/Favorites")]
    public static void ShowWindow()
    {
        GetWindow<FavoritesWindow>("Favorites");
    }

    private void OnEnable()
    {
        LoadFavorites();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("★ Favorite Assets", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        for (int i = 0; i < favoriteGuids.Count; i++)
        {
            string guid = favoriteGuids[i];
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Object asset = AssetDatabase.LoadAssetAtPath<Object>(path);

            if (asset == null)
                continue;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(asset.name, GUILayout.ExpandWidth(true)))
            {
                Selection.activeObject = asset;
                EditorGUIUtility.PingObject(asset);
            }

            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                favoriteGuids.RemoveAt(i);
                SaveFavorites();
                break;
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();
        if (Selection.activeObject != null)
        {
            if (GUILayout.Button($" Add '{Selection.activeObject.name}' to Favorites"))
            {
                string selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);
                string guid = AssetDatabase.AssetPathToGUID(selectedPath);
                if (!favoriteGuids.Contains(guid))
                {
                    favoriteGuids.Add(guid);
                    SaveFavorites();
                }
            }
        }
    }

    private void LoadFavorites()
    {
        favoriteGuids.Clear();
        string data = EditorPrefs.GetString(prefsKey, "");
        if (!string.IsNullOrEmpty(data))
        {
            favoriteGuids.AddRange(data.Split('|'));
        }
    }

    private void SaveFavorites()
    {
        string data = string.Join("|", favoriteGuids);
        EditorPrefs.SetString(prefsKey, data);
    }
}
