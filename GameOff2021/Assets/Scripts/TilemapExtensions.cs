using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TilemapExtensions
{
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
