using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Level")]
public class Level : ScriptableObject
{
    public GameObject Tilemap;
    public List<MultiTile> Pieces;

    public int PieceCount()
    {
        if (Pieces == null) return 0;
        return Pieces.Count;
    }
}
