using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelLoader : MonoBehaviour
{

    [SerializeField] Tilemap levelLayer;
    [SerializeField] Level debugLevel;
    [SerializeField] bool debug;

    private void Start()
    {

        if (debug)
        {
            LoadLevel(debugLevel);
        }
        else
        {
            LoadLevel(GameManager.Instance.CurrentLevel);
        }
    }

    //copying a lot of this from LevelEditorEditor
    public void LoadLevel(Level level)
    {
        Tilemap levelMap = level.Tilemap.GetComponent<Tilemap>();
        levelLayer.ClearAllTiles();
        TileBase[] alltiles = levelMap.GetTilesBlock(levelMap.cellBounds);
        levelLayer.SetTilesBlock(levelMap.cellBounds, alltiles);

        setPiecesFromLevel(level);
    }

    public void OnLevelCompleteHandler()
    {
        Timer timer = new Timer(this, 2);
        timer.OnTimerCompleted += LoadNextLevel;
        timer.Start();
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
            PiecesManager.Instance.AddPiece(tile, tile.EditorPosition);
            //PiecesManager.Instance.AddPiece(tile, Vector3.zero);
        }
    }
}
