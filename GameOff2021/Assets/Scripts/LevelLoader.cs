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
    public Level currentLevel;

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
        currentLevel = levels[0];
        levels.RemoveAt(0);
        currentLevelLayer.ClearAllTiles();
        Tilemap levelMap = currentLevel.Tilemap.GetComponent<Tilemap>();
        TileBase[] alltiles = levelMap.GetTilesBlock(levelMap.cellBounds);
        currentLevelLayer.SetTilesBlock(levelMap.cellBounds, alltiles);
        List<Tilemap> pieces = new List<Tilemap>();

        RemoveAllPieces();

        for (int i = 0; i < currentLevel.PieceCount(); i++)
        {
            pieces.Add(Instantiate(pieceLayer, Vector3.zero, Quaternion.identity));
        }

        for(int i = 0; i < pieces.Count; i++)
        {
            pieces[i].transform.SetParent(pieceLayer.transform);
            pieces[i].ApplyMultiTileAt(currentLevel.Pieces[i], currentLevel.Pieces[i].EditorPosition);
        }
    }

    private void RemoveAllPieces()
    {
        List<GameObject> toDestroy = new List<GameObject>();
        foreach (Transform child in pieceLayer.transform)
            toDestroy.Add(child.gameObject);

        pieceLayer.transform.DetachChildren();
        foreach (GameObject g in toDestroy)
            Destroy(g);
    }

    public void ReloadLevel()
    {
        currentLevelLayer.ClearAllTiles();
        Tilemap levelMap = currentLevel.Tilemap.GetComponent<Tilemap>();
        TileBase[] alltiles = levelMap.GetTilesBlock(levelMap.cellBounds);
        currentLevelLayer.SetTilesBlock(levelMap.cellBounds, alltiles);
        List<Tilemap> pieces = new List<Tilemap>();

        RemoveAllPieces();

        for (int i = 0; i < currentLevel.PieceCount(); i++)
        {
            pieces.Add(Instantiate(pieceLayer, Vector3.zero, Quaternion.identity));
        }

        for (int i = 0; i < pieces.Count; i++)
        {
            pieces[i].transform.SetParent(pieceLayer.transform);
            pieces[i].ApplyMultiTileAt(currentLevel.Pieces[i], currentLevel.Pieces[i].EditorPosition);
        }
    }
}
