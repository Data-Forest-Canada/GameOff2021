using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// This tile applies given recipes to it's surrounding tiles 
// NOTE: the given recipes should not contain recipes with the SAME input but DIFFERENT outputs. This will cause unpredictable behaviour
[CreateAssetMenu(menuName = "GameTile/RadialPlace")]
public class RadialPlaceGameTile : GameTile
{
    [SerializeField] TileRecipe[] recipesToApply;

    public override void OnPlace(Tilemap map, Vector3Int position)
    {
        GameTile[] surroundingTiles = map.GetSurroundingGameTiles(position);
        GameTile[] tilesToSet = new GameTile[6];

        // Looping through each tile and recipe, trying each recipe on each tile.
        for (int i = 0; i < surroundingTiles.Length; i++)
        {
            // Set it to the current tile since the applying function would override.
            tilesToSet[i] = surroundingTiles[i];

            foreach (TileRecipe recipe in recipesToApply)
            {
                GameTile combinedTile = recipe.Combine(surroundingTiles[i]);

                if (combinedTile != null)
                {
                    tilesToSet[i] = combinedTile;
                    break; //  Breaking here to save on iterations. We shouldn't be using recipes that make different outputs. 
                }
            }
        }

        map.SetSurroundingGameTiles(position, tilesToSet);
    }
}
