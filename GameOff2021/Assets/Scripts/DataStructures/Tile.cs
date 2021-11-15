using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tile")]
public class Tile : UnityEngine.Tilemaps.Tile
{
    [SerializeField] TileType type;
    public TileType Type
    {
        get { return type; }
        private set { Type = value; }
    }

    [SerializeField] TileRecipe[] recipes;

    // Utilizing a recipe system, we can add or remove combinations modularly.
    // This way we don't need to define a new class for every type of tile and manually code each interation.
    public virtual Tile CombineWith(Tile tile)
    {
        foreach (TileRecipe recipe in recipes)
        {
            if (recipe.CanCombine(tile))
            {
                return recipe.Combine(tile);
            }
        }

        return null;
    }

    // Some tiles may have a unique effect upon being placed.
    // Providing a standard implementation of nothing happening (for convenience) as I believe that will be the majority of cases.
    // Unique OnPlace tiles should be derived from the base class (Unless we think of a way to modularly do that?)
    public virtual void OnPlace(Grid grid, Vector3Int position)
    {
        return;
    }
}
