using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

[CustomEditor(typeof(LevelEditor))]
public class LevelEditorEditor : Editor
{
    Level level;
    LevelEditor editor;

    private void OnEnable()
    {
        // Get the editor and cache it
        editor = (LevelEditor)target;

        // Take the level if provided
        level = editor.Level;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Save Level"))
        {
            if (level == null)
            {
                // Create and link it to the editor
                level = ScriptableObject.CreateInstance<Level>();
                editor.Level = level;

                fillLevel();

                // Create a path and then save
                string path = AssetDatabase.GenerateUniqueAssetPath("Assets/Levels/NewLevel.asset");
                AssetDatabase.CreateAsset(level, path);
            }
            else
            {
                fillLevel();
                AssetDatabase.SaveAssets();
            }

            Debug.Log(AssetDatabase.GetAssetPath(level));
        }
    }

    void fillLevel()
    {
        string path = getOrCreateFolderAt("NewLevelPrefabs");
        Debug.Log(path);
        level.Tilemap = createPrefab(editor.Tilemap.gameObject, path, "NewLevelTilemap");
    }

    GameObject createPrefab(GameObject go, string path, string prefabName)
    {
        string prefabPath = AssetDatabase.GenerateUniqueAssetPath(path + "/" + prefabName + ".prefab");
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, prefabPath, out bool success);

        return success ? prefab : null;
    }

    string getOrCreateFolderAt(string folderName)
    {
        // Check if the folder exists
        string path = "Assets/Levels/" + folderName;
        if (AssetDatabase.IsValidFolder(path)) return path;

        // if not, create the file
        AssetDatabase.CreateFolder("Assets/Levels", folderName);
        return path;
    }
}
