using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TilemapExtensions
{
    // Returns the surrounding tiles of a hex tilemap starting from the left in a clockwise pattern.
    public static GameTile[] GetSurroundingGameTiles(this Tilemap map, Vector3Int position)
    {
        GameTile[] tiles = new GameTile[5];

        tiles[0] = (GameTile)map.GetTile(position + Vector3Int.left);
        tiles[1] = (GameTile)map.GetTile(position + Vector3Int.up);
        tiles[2] = (GameTile)map.GetTile(position + Vector3Int.up + Vector3Int.right);
        tiles[3] = (GameTile)map.GetTile(position + Vector3Int.right);
        tiles[4] = (GameTile)map.GetTile(position + Vector3Int.down + Vector3Int.right);
        tiles[5] = (GameTile)map.GetTile(position + Vector3Int.down);

        return tiles;
    }

    public static void SetSurroundingGameTiles(this Tilemap map, Vector3Int position, GameTile[] tiles)
    {
        if (tiles.Length < 5) Debug.Log("tiles must be at least of length 5");

        map.SetTile(position + Vector3Int.left, tiles[0]);
        map.SetTile(position + Vector3Int.up, tiles[1]);
        map.SetTile(position + Vector3Int.up + Vector3Int.right, tiles[2]);
        map.SetTile(position + Vector3Int.right, tiles[3]);
        map.SetTile(position + Vector3Int.down + Vector3Int.right, tiles[4]);
        map.SetTile(position + Vector3Int.down, tiles[5]);
    }

    public static GameTile[] GetAllGameTiles(this Tilemap map)
    {
        BoundsInt.PositionEnumerator enu = map.cellBounds.allPositionsWithin;
        List<GameTile> gameTiles = new List<GameTile>();
        GameTile current;
        while (enu.MoveNext())
        {
            current = map.GetTile<GameTile>(enu.Current);
            if (current != null)
                gameTiles.Add(current);
        }

        return gameTiles.ToArray();
    }

    public static void ApplyMultiTileAt(this Tilemap map, MultiTile tile, Vector3Int position)
    {
        Vector3Int[] positions = tile.Positions();

        // Our job here is done B)
        if (positions.Length == 0) return;

        Vector3Int delta = position - positions[0];

        bool originIsOdd = positions[0].y % 2 == 1;
        bool givenIsOdd = position.y % 2 == 1;

        bool correctionNeeded = originIsOdd != givenIsOdd;

        for (int i = 0; i < positions.Length; i++)
        {
            // Even to even or odd to odd movement
            if (correctionNeeded)
            {
                // Shift right
                if (originIsOdd && positions[i].y % 2 == 0)
                {
                    positions[i] += delta + new Vector3Int(-1, 0, 0);
                }
                // Shift left
                else if (!originIsOdd && positions[i].y % 2 == 1)
                {
                    positions[i] += delta + new Vector3Int(1, 0, 0);
                }
                else
                {
                    positions[i] += delta;
                }
            }
            else
            {
                positions[i] += delta;
            }
        }

        map.SetTiles(positions, tile.Tiles());
    }
}
