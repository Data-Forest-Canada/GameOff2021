using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "MultiTile")]
public class MultiTile : ScriptableObject
{

    [System.Serializable]
    class OffsetTile
    {
        public Vector3Int offset;
        public Tile tile;
    }

    [SerializeField] OffsetTile[] offsetTiles;

    public Vector3Int[] Positions() => offsetTiles.Select(tile => tile.offset).ToArray();

    public Tile[] Tiles() => offsetTiles.Select(tile => tile.tile).ToArray();

    public Tilemap toTilemap()
    {
        Tilemap map = new Tilemap();    //we'd probably instantiate a prefab or something
        map.SetTiles(Positions(), Tiles());
        return map;
    }
}
