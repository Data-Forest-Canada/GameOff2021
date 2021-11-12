using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Recipe")]
public class TileRecipe : ScriptableObject
{
    [SerializeField] Tile input;
    [SerializeField] Tile outPut;

    public bool CanCombine(Tile tile)
    {
        return tile.Type == input.Type;
    }

    public Tile Combine(Tile tile)
    {
        if (CanCombine(tile)) return outPut;
        return null;
    }
}
