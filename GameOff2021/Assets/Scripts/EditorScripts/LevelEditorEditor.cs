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
        if (GUILayout.Button("New Level"))
        {
            newLevel();
        }
        else if (GUILayout.Button("Save Level"))
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

    // ----------------------------------------------------------------------------------------------

    void newLevel()
    {
        // Create and link it to the editor
        level = ScriptableObject.CreateInstance<Level>();
        editor.Level = level;

        editor.ResetPieces();

        loadLevel();
    }

    void loadLevel()
    {
        if (level == null)
        {
            Debug.Log("Please assign a level to the LevelEditor before loading");
            return;
        }

        editor.Tilemap.ClearAllTiles();

        if (level.Tilemap != null)
        {

            Tilemap levelMap = level.Tilemap.GetComponent<Tilemap>();
            TileBase[] allTiles = levelMap.GetTilesBlock(levelMap.cellBounds);

            editor.Tilemap.SetTilesBlock(levelMap.cellBounds, allTiles);
        }

        for (int i = 0; i < level.PieceCount(); i++)
        {
            if (i >= editor.Pieces.Count)
            {
                makeNewPiece();
            }

            Tilemap map = editor.Pieces[i].GetComponent<Tilemap>();
            MultiTile multiTile = level.Pieces[i];

            map.ClearAllTiles();
            map.ApplyMultiTileAt(multiTile, multiTile.EditorPosition);
        }
    }

    void makeNewPiece()
    {
        editor.AddPiece();
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
            Debug.Log(path);
            AssetDatabase.CreateAsset(level, path);
        }
        else
        {
            fillLevel();
            AssetDatabase.SaveAssets();
        }
    }

    // ----------------------------------------------------------------------------------------------

    void fillLevel()
    {
        string path = getOrCreateFolderAt("NewLevelPrefabs");
        bool overwrite = level.Tilemap != null;
        level.Tilemap = createPrefab(editor.Tilemap.gameObject, path, "NewLevelTilemap", overwrite);
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

    GameObject createPrefab(GameObject go, string path, string prefabName, bool overwrite)
    {
        string prefabPath = path + "/" + prefabName + ".prefab";

        if (!overwrite)
        {
            prefabPath = AssetDatabase.GenerateUniqueAssetPath(prefabPath);
        }
        else
        {
            prefabPath = AssetDatabase.GetAssetPath(level.Tilemap);
        }

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
        // Create a temporary multitile for the purpose of only collecting the tiles in the map as they are
        MultiTile temp = ScriptableObject.CreateInstance<MultiTile>();

        map.CompressBounds();

        BoundsInt bounds = map.cellBounds;

        // Hold onto the originals to reset at the end
        TileBase[] original = map.GetTilesBlock(bounds);

        bool firstTileNotFound = true;

        // Loop and take the piece as is, taking the first tile's position as the editor position
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
                        temp.EditorPosition = position;
                        firstTileNotFound = false;
                    }

                    temp.AddTile(tileAt, position);
                }
            }
        }

        // Clear the map and then apply the temp tile at the origin
        map.ClearAllTiles();
        map.ApplyMultiTileAt(temp, Vector3Int.zero);

        // This is to actually save the multitile
        MultiTile final = ScriptableObject.CreateInstance<MultiTile>();

        map.CompressBounds();
        BoundsInt newBounds = map.cellBounds;
        final.EditorPosition = temp.EditorPosition;

        // Collect the tiles at their new positions
        for (int x = newBounds.xMin; x <= newBounds.xMax; x++)
        {
            for (int y = newBounds.yMin; y <= newBounds.yMax; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                GameTile tileAt = (GameTile)map.GetTile(position);

                if (tileAt != null) final.AddTile(tileAt, position);
            }
        }

        // Reset the tiles
        map.ClearAllTiles();
        map.SetTilesBlock(bounds, original);

        return final;
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
