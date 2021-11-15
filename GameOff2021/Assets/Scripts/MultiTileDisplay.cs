using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class MultiTileDisplay : MonoBehaviour
{
    public MultiTile Tile;
    [SerializeField] Tilemap tilemap;

    private void Awake()
    {
        Apply();
    }

    public void Apply()
    {
        if (tilemap == null)
        {
            tilemap = GetComponent<Tilemap>();
        }

        Tile.SetTilemap(tilemap);
    }
}
