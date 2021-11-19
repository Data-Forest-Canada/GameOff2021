using System.Collections;
using System.Text;
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
        public GameTile Tile;

        public OffsetTile(GameTile tile, Vector3Int position)
        {
            Tile = tile;
            Offset = position;
        }
    }

    public List<OffsetTile> OffsetTiles = new List<OffsetTile>();
    public Vector3Int EditorPosition;

    public Vector3Int[] Positions() => OffsetTiles.Select(tile => tile.Offset).ToArray();
    public GameTile[] Tiles() => OffsetTiles.Select(tile => tile.Tile).ToArray();

    public void AddTile(GameTile tile, Vector3Int position) => OffsetTiles.Add(new OffsetTile(tile, position));

    // Instead of making a tilemap, I changed this to set one instead. 
    // Creating a component and adding it programatically to gameobject is more compicated and this is an easier way. 
    public void SetTilemap(Tilemap tilemap)
    {
        tilemap.SetTiles(Positions(), Tiles());
    }

    // This is a way to see if two multi tiles are functionally the same.
    // An equal multi tile contains the same tiles in the same positions but not necessarily the same order in the list.
    public bool Equals(MultiTile tileToCompare)
    {
        Dictionary<Vector3Int, GameTile> otherTiles = mapToDictionary(tileToCompare);
        Dictionary<Vector3Int, GameTile> ourTiles = mapToDictionary(this);

        foreach (Vector3Int position in otherTiles.Keys)
        {
            // If we don't have a key or the key doesnt map to the same value then the tile is not equal
            if (!ourTiles.ContainsKey(position)) return false;
            if (ourTiles[position] != otherTiles[position]) return false;
        }

        foreach (Vector3Int position in ourTiles.Keys)
        {
            // If they don't have a key or the key doesnt map to the same value then the tile is not equal
            if (!otherTiles.ContainsKey(position)) return false;
            if (otherTiles[position] != ourTiles[position]) return false;
        }

        return true;
    }

    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();

        foreach (OffsetTile tile in OffsetTiles)
        {
            builder.Append(tile.Tile.Type.ToString() + "At" + tile.Offset.ToString() + ",");
        }

        return builder.ToString();
    }

    Dictionary<Vector3Int, GameTile> mapToDictionary(MultiTile given)
    {
        Dictionary<Vector3Int, GameTile> tiles = new Dictionary<Vector3Int, GameTile>();

        foreach (OffsetTile tile in given.OffsetTiles)
        {
            tiles.Add(tile.Offset, tile.Tile);
        }

        return tiles;
    }
}
