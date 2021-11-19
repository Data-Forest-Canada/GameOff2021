using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.BoundsInt;

[RequireComponent(typeof(Grid))]
public class DragAndDrop : MonoBehaviour
{
    [SerializeField] GameObject pieceLayer;
    [SerializeField] Tilemap levelTilemap;
    Grid pointerGrid;
    Vector2 mousePosition;
    public Tilemap selected;
    Dictionary<Vector3Int, GameTile> original = new Dictionary<Vector3Int, GameTile>();

    bool active() => Input.GetMouseButton(0);

    private void Start()
    {
        pointerGrid = GetComponent<Grid>();
        levelTilemap.CompressBounds();
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
            //Debug.Log($"map {i}; {cellPos}");
            tile = map.GetTile<GameTile>(cellPos);
            if (tile != null)
            {
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
        foreach(Vector3Int pos in piecePositions)
        {
            levelPos = levelTilemap.WorldToCell(selected.CellToWorld(pos));
            if (!levelTilemap.HasTile(levelPos))
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
            foreach(Vector3Int pos in piecePositions)
            {
                levelPos = levelTilemap.WorldToCell(selected.CellToWorld(pos));
                current = levelTilemap.GetTile<GameTile>(levelPos);
                newTile = current.CombineWith(original[pos]);
                levelTilemap.SetTile(levelPos, newTile ?? current);
            }
            Destroy(selected.gameObject);
        }
        else
        {
            selected.transform.SetParent(pieceLayer.transform);
        }
        selected = null;
        original.Clear();
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

    private void Update()
    {

        if(active() && selected == null)
        {
            if(PickUp())
                StoreOriginals();
        }

        if(active() && selected != null)
        {
            List<Vector3Int> piecePositions;
            if (OnLevelMap(out piecePositions))
            {
                Preview(piecePositions);
            }
            else
            {
                foreach(Vector3Int pos in piecePositions)
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

        if(selected != null && !active())
        {
            Release();
        }

        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousePosition.x, mousePosition.y, -1);
    }
}
