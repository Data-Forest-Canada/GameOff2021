using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "MultiTile")]
public class MultiTile : ScriptableObject
{

    [System.Serializable]
    public class OffsetTile
    {
        public Vector3Int Offset;
        public Tile Tile;

        public OffsetTile(Tile tile, Vector3Int position)
        {
            Tile = tile;
            Offset = position;
        }
    }

    public List<OffsetTile> OffsetTiles = new List<OffsetTile>();

    public Vector3Int[] Positions() => OffsetTiles.Select(tile => tile.Offset).ToArray();

    public Tile[] Tiles() => OffsetTiles.Select(tile => tile.Tile).ToArray();
    public void AddTile(Tile tile, Vector3Int position) => OffsetTiles.Add(new OffsetTile(tile, position));

    // Instead of making a tilemap, I changed this to set one instead. 
    // Creating a component and adding it programatically to gameobject is more compicated and this is an easier way. 
    public void SetTilemap(Tilemap tilemap)
    {
        tilemap.SetTiles(Positions(), Tiles());
    }

    public bool CanBePlacedAt(Tilemap tilemap, Vector3Int position)
    {
        // TODO
        return false;
    }

    // This is a way to see if two multi tiles are functionally the same.
    // An equal multi tile contains the same tiles in the same positions but not necessarily the same order in the list.
    public bool Equals(MultiTile tileToCompare)
    {
        Dictionary<Vector3Int, Tile> otherTiles = mapToDictionary(tileToCompare);
        Dictionary<Vector3Int, Tile> ourTiles = mapToDictionary(this);

        foreach (Vector3Int position in otherTiles.Keys)
        {
            // If we don't have a key or the key doesnt map to the same value then the tile is not equal
            if (!ourTiles.ContainsKey(position)) return false;
            if (ourTiles[position] != otherTiles[position]) return false;
        }

        return true;
    }

    Dictionary<Vector3Int, Tile> mapToDictionary(MultiTile given)
    {
        Dictionary<Vector3Int, Tile> tiles = new Dictionary<Vector3Int, Tile>();

        foreach (OffsetTile tile in given.OffsetTiles)
        {
            tiles.Add(tile.Offset, tile.Tile);
        }

        return tiles;
    }
}
