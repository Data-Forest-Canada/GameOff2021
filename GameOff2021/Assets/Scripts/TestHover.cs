using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestHover : MonoBehaviour
{
    [SerializeField] bool testing;
    [SerializeField] Tilemap map;

    [SerializeField] TileBase tileBase;

    Vector3Int last;

    // Update is called once per frame
    void Update()
    {
        Vector3Int current = MousePositionToCellPosition.CellHovered;

        if (last == null)
        {
            map.SetTile(current, tileBase);
            last = current;
        }

        if (testing && current != last)
        {
            map.SetTile(current, tileBase);
            map.SetTile(last, null);

            last = current;
        }
    }
}
