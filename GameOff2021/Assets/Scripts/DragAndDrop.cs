using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.BoundsInt;
using UnityEngine.Events;

[RequireComponent(typeof(Grid))]
public class DragAndDrop : MonoBehaviour
{

    [SerializeField] GameObject pieceLayer;
    [SerializeField] Tilemap levelTilemap;
    Grid pointerGrid;
    Vector2 mousePosition;
    public Tilemap selected;
    public UnityEvent OnLevelComplete;
    Dictionary<Vector3Int, GameTile> original = new Dictionary<Vector3Int, GameTile>();
    Timer ticker;
    Vector3 originalPosition;
    bool preview = false;

    bool active() => Input.GetMouseButton(0);

    private void Start()
    {
        pointerGrid = GetComponent<Grid>();
        levelTilemap.CompressBounds();
        ticker = new Timer(this, 1);
        ticker.OnTimerCompleted += TogglePreview;
        ticker.Start();
    }

    private bool PickUp()
    {
        Tilemap[] maps = pieceLayer.GetComponentsInChildren<Tilemap>();
        GameTile tile;
        Vector3Int cellPos;
        int i = 0;

        foreach (Tilemap map in maps)
        {
            cellPos = map.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            cellPos = new Vector3Int(cellPos.x, cellPos.y, 0);
            cellPos = new Vector3Int(cellPos.x, cellPos.y, 0);
            //Debug.Log($"map {i}; {cellPos}");
            tile = map.GetTile<GameTile>(cellPos);
            if (tile != null)
            {
                originalPosition = map.transform.position;
                //Debug.Log(tile);
                map.transform.SetParent(transform);
                selected = map;
                return true;
            }

            i++;
        }

        return false;
    }

    private bool OnLevelMap(out List<Vector3Int> piecePositions)
    {
        PositionEnumerator pieceEnu = selected.cellBounds.allPositionsWithin;
        piecePositions = new List<Vector3Int>();
        while (pieceEnu.MoveNext())
        {
            if (selected.HasTile(pieceEnu.Current))
                piecePositions.Add(pieceEnu.Current);
        }

        Vector3Int levelPos;
        foreach (Vector3Int pos in piecePositions)
        {
            levelPos = levelTilemap.WorldToCell(selected.CellToWorld(pos));
            if (!levelTilemap.HasTile(levelPos))
                return false;
        }
        return true;
    }

    private bool LevelComplete()
    {
        GameTile[] gameTiles = levelTilemap.GetAllGameTiles();
        foreach (GameTile tile in gameTiles)
        {
            if (tile.Type != TileType.DRONE)
                return false;
        }

        return true;
    }

    private void Release()
    {
        List<Vector3Int> piecePositions;
        if (OnLevelMap(out piecePositions))
        {
            Vector3Int levelPos;
            GameTile current, newTile;
            Dictionary<Vector3Int, GameTile> transaction = new Dictionary<Vector3Int, GameTile>();
            foreach (Vector3Int pos in piecePositions)
            {
                //match the current tile position to the level grid
                levelPos = levelTilemap.WorldToCell(selected.CellToWorld(pos));

                //get the tile from the level that matches the current tile position
                current = levelTilemap.GetTile<GameTile>(levelPos);


                //get the combined tile 
                newTile = current.CombineWith(original[pos]);

                //store the new tile as a transaction
                transaction.Add(levelPos, newTile);

                // If we hit a null tile we can just abort. We reset the piece positionm before we go
                if (newTile == null)
                {
                    selected.transform.SetParent(pieceLayer.transform);
                    selected.transform.position = originalPosition;

                    return;
                }
            }

            // Only apply once we have a full list with no nulls
            foreach (KeyValuePair<Vector3Int, GameTile> entry in transaction)
            {
                levelTilemap.SetTile(entry.Key, entry.Value);
                entry.Value.OnPlace(levelTilemap, entry.Key);
            }

            Destroy(selected.gameObject);
        }
        else
        {
            selected.transform.SetParent(pieceLayer.transform);
        }
        selected = null;
        original.Clear();
        // Previously this was checking to see if you used all your pieces. I don't think we want that to be a condition of winning so I removed it
        if (LevelComplete())
        {
            GameManager.Instance.UnlockNextLevel();
            OnLevelComplete?.Invoke();
        }
    }

    private void Preview(List<Vector3Int> piecePositions)
    {
        Vector3Int levelPos;
        GameTile current, newTile;
        foreach (Vector3Int pos in piecePositions)
        {
            levelPos = levelTilemap.WorldToCell(selected.CellToWorld(pos));
            current = levelTilemap.GetTile<GameTile>(levelPos);
            //newTile = current.CombineWith(selected.GetTile<GameTile>(pos));
            newTile = current.CombineWith(original[pos]);
            //if(current.Type != TileType.NONE)
            //    Debug.Log($"{newTile}, {pos}");
            selected.SetTile(pos, newTile ?? original[pos]);

        }
    }

    private void StoreOriginals()
    {
        PositionEnumerator pieceEnu = selected.cellBounds.allPositionsWithin;
        while (pieceEnu.MoveNext())
        {
            if (selected.HasTile(pieceEnu.Current))
            {
                original.Add(pieceEnu.Current, selected.GetTile<GameTile>(pieceEnu.Current));
                //Debug.Log(pieceEnu.Current);
            }

        }
    }

    private void TogglePreview()
    {
        if (!preview)
            preview = true;
        else
            preview = false;

        ticker.Restart();
    }

    private void Update()
    {
        if (active() && selected == null)
        {
            if (PickUp())
                StoreOriginals();
        }

        if (active() && selected != null)
        {
            List<Vector3Int> piecePositions;
            if (OnLevelMap(out piecePositions))
            {
                if (ticker.Paused)
                {
                    preview = true;
                    ticker.Restart();
                    ticker.Paused = false;

                }

                if (preview)
                    Preview(piecePositions);
                else
                {
                    foreach (Vector3Int pos in piecePositions)
                    {
                        selected.SetTile(pos, original[pos]);
                    }
                }

            }
            else
            {
                ticker.Paused = true;
                foreach (Vector3Int pos in piecePositions)
                {
                    selected.SetTile(pos, original[pos]);
                }
                //Dictionary<Vector3Int, GameTile>.Enumerator enu = original.GetEnumerator();
                //while (enu.MoveNext())
                //{
                //    selected.SetTile(enu.Current.Key, enu.Current.Value);
                //}
            }

        }

        if (selected != null && !active())
        {
            ticker.Paused = true;
            Release();
        }

        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousePosition.x, mousePosition.y, -1);
    }
}
