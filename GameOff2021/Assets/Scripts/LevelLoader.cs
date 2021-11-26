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
    public int currentLevelIndex;

    private void Start()
    {
        currentLevelIndex = 0;

        DragAndDrop.OnLevelComplete += LoadNextLevel;
        hasEventHandler = true;
        CompletedLevelLayers = new List<Tilemap>();

        LoadLevel(levels[currentLevelIndex]);
    }

    //copying a lot of this from LevelEditorEditor
    public void LoadLevel(Level level)
    {
        currentLevelLayer.ClearAllTiles();
        Tilemap levelMap = level.Tilemap.GetComponent<Tilemap>();
        TileBase[] alltiles = levelMap.GetTilesBlock(levelMap.cellBounds);
        currentLevelLayer.SetTilesBlock(levelMap.cellBounds, alltiles);
        List<Tilemap> pieces = new List<Tilemap>();

        setPiecesFromLevel(level);
    }

    public void LoadNextLevel()
    {
        currentLevelIndex++;
        LoadLevel(levels[currentLevelIndex]);
    }

    public void ReloadLevel()
    {
        LoadLevel(levels[currentLevelIndex]);
    }

    void setPiecesFromLevel(Level level)
    {
        PiecesManager.Instance.ResetPieces();

        foreach (MultiTile tile in level.Pieces)
        {
            PiecesManager.Instance.AddPiece(tile, Vector3.zero);
        }
    }
}
