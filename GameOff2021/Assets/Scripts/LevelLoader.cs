using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class LevelLoader : MonoBehaviour
{
    public List<Level> levels;
    public List<Tilemap> CompletedLevelLayers;
    public Tilemap currentLevelLayer;
    public Tilemap pieceLayer;
    private static bool hasEventHandler = false;

    private void Start()
    {
        DragAndDrop.OnLevelComplete += LoadLevel;
        hasEventHandler = true;
        CompletedLevelLayers = new List<Tilemap>();
        LoadLevel();
    }

    //copying a lot of this from LevelEditorEditor
    public void LoadLevel()
    {
        Level toLoad = levels[0];
        levels.RemoveAt(0);
        currentLevelLayer.ClearAllTiles();
        Tilemap levelMap = toLoad.Tilemap.GetComponent<Tilemap>();
        TileBase[] alltiles = levelMap.GetTilesBlock(levelMap.cellBounds);
        currentLevelLayer.SetTilesBlock(levelMap.cellBounds, alltiles);
        List<Tilemap> pieces = new List<Tilemap>();

        for (int i = 0; i < toLoad.PieceCount(); i++)
        {
            pieces.Add(Instantiate(pieceLayer, Vector3.zero, Quaternion.identity));
        }

        for(int i = 0; i < pieces.Count; i++)
        {
            pieces[i].transform.SetParent(pieceLayer.transform);
            pieces[i].ApplyMultiTileAt(toLoad.Pieces[i], toLoad.Pieces[i].EditorPosition);
        }
    }
}
