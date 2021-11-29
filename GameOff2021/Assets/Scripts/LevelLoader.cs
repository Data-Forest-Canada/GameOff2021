using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class LevelLoader : MonoBehaviour
{

    public Tilemap currentLevelLayer;
    public Tilemap pieceLayer;

    private void Start()
    {
        DragAndDrop.OnLevelComplete += LoadNextLevel;

        LoadLevel(GameManager.Instance.CurrentLevel);
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
        LoadLevel(GameManager.Instance.NextUnlockedLevel);
    }

    public void ReloadLevel()
    {
        LoadLevel(GameManager.Instance.CurrentLevel);
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
