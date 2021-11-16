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
            saveLevel();
        }
        else if (GUILayout.Button("Create New Piece"))
        {
            makeNewPiece();
        }
        else if (GUILayout.Button("Load Level"))
        {
            loadLevel();
        }
    }

    void loadLevel()
    {
        if (level == null)
        {
            Debug.Log("Please assign a level to the LevelEditor before loading");
            return;
        }

        Tilemap levelMap = level.Tilemap.GetComponent<Tilemap>();
        TileBase[] allTiles = levelMap.GetTilesBlock(levelMap.cellBounds);

        editor.Tilemap.ClearAllTiles();
        editor.Tilemap.SetTilesBlock(levelMap.cellBounds, allTiles);

        for (int i = 0; i < level.Pieces.Count; i++)
        {
            if (i > editor.Pieces.Count - 1)
            {
                makeNewPiece();
            }

            Tilemap map = editor.Pieces[i].GetComponent<Tilemap>();
            MultiTile tile = level.Pieces[i];

            map.ClearAllTiles();
            tile.SetTilemap(map);
        }
    }

    void makeNewPiece()
    {
        GameObject newTilemapObject = Instantiate(editor.PiecePrefab, Vector3.zero, Quaternion.identity, editor.TilemapParent);
        editor.AddPiece(newTilemapObject);
    }

    void saveLevel()
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

    void fillLevel()
    {
        string path = getOrCreateFolderAt("NewLevelPrefabs");
        level.Tilemap = createPrefab(editor.Tilemap.gameObject, path, "NewLevelTilemap");
        List<MultiTile> pieces = convertPiecesToMultiTiles();
        level.Pieces = pieces;
    }

    List<MultiTile> convertPiecesToMultiTiles()
    {
        List<MultiTile> pieces = new List<MultiTile>();

        for (int i = 0; i < editor.Pieces.Count; i++)
        {
            Tilemap map = editor.Pieces[i].GetComponent<Tilemap>();

            MultiTile newTile = getOrCreateMultiTileFrom(map);
            pieces.Add(newTile);
        }

        return pieces;
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

    // Makes a Multitile out of a tilemap, or gets one if the conversion already exists as an asset
    MultiTile getOrCreateMultiTileFrom(Tilemap map)
    {
        MultiTile tile = convertTilemapToMultiTile(map);
        tile = saveMultiTile(tile);
        return tile;
    }


    MultiTile saveMultiTile(MultiTile tile)
    {
        if (multiTileExistsInAssets(tile, out MultiTile existingTile))
        {
            Debug.Log("Multitile already exists!");
            return existingTile;
        }
        else
        {
            string folderPath = getOrCreateFolderAt("MultTiles");
            // Create a path and then save
            string filePath = AssetDatabase.GenerateUniqueAssetPath(folderPath + "/NewMultiTile.asset");
            AssetDatabase.CreateAsset(tile, filePath);

            return tile;
        }
    }

    MultiTile convertTilemapToMultiTile(Tilemap map)
    {
        MultiTile newMultiTile = ScriptableObject.CreateInstance<MultiTile>();

        map.CompressBounds();

        BoundsInt bounds = map.cellBounds;


        Vector3Int pivot = Vector3Int.zero;
        // Need to use two variables because Vector3Int is non-nullable;
        bool firstTileNotFound = true;

        for (int x = bounds.xMin; x <= bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y <= bounds.yMax; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                GameTile tileAt = (GameTile)map.GetTile(position);

                if (tileAt != null)
                {
                    if (firstTileNotFound)
                    {
                        firstTileNotFound = false;
                        pivot = position;
                    }

                    newMultiTile.AddTile(tileAt, position - pivot);
                }
            }
        }

        return newMultiTile;
    }

    bool multiTileExistsInAssets(MultiTile givenTile, out MultiTile existingTile)
    {
        string[] guids = AssetDatabase.FindAssets("t:MultiTile");

        foreach (string guid in guids)
        {
            MultiTile tile = AssetDatabase.LoadAssetAtPath<MultiTile>(AssetDatabase.GUIDToAssetPath(guid));
            if (tile.Equals(givenTile))
            {
                existingTile = tile;
                return true;
            }
        }

        existingTile = givenTile;
        return false;
    }
}
