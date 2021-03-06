using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Recipe")]
public class TileRecipe : ScriptableObject
{
    [SerializeField] GameTile input;
    [SerializeField] GameTile outPut;

    public bool CanCombine(GameTile tile)
    {
        if (tile == null) return false;
        if (input == null) return true;
        return tile.Type == input.Type;
    }

    public GameTile Combine(GameTile tile)
    {
        if (CanCombine(tile)) return outPut;
        return null;
    }
}
