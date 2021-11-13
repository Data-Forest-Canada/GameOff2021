using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PlacesTiles : MonoBehaviour
{
    public Tile toPlace;
    public Tilemap tilemap;
    public GameObject completionText;
    

    public void ChangeTileToPlace(Tile tile)
    {
        toPlace = tile;
    }

    public void PlaceTileAtCell(Vector3Int cell)
    {
        //Debug.Log(tilemap.GetTile<Tile>(cell));
        //Debug.Log(toPlace);
        //Debug.Log(cell);
        Tile atLocation = tilemap.GetTile<Tile>(cell);
        Tile newTile = atLocation?.CombineWith(toPlace) ?? atLocation;
        tilemap.SetTile(cell, newTile);


        //Debug.Log($"{toPlace} + {atLocation} = {newTile}");

        if (CheckCompletion())
            completionText.SetActive(true);

    }

    public bool CheckCompletion()
    {
        BoundsInt.PositionEnumerator pos = tilemap.cellBounds.allPositionsWithin;
        while (pos.MoveNext())
        {
            if (tilemap.HasTile(pos.Current))
            {
                if (tilemap.GetTile<Tile>(pos.Current)?.Type != TileType.DRONE)
                {
                    Debug.Log(pos.Current);
                    Debug.Log(tilemap.GetTile(pos.Current));
                    Debug.Log(tilemap.GetTile<Tile>(pos.Current)?.Type);
                    return false;
                }
                    
            }
        }
        return true;
    }

    bool isLeftClicking() => Input.GetMouseButtonDown(0);
    bool active = false;

    float delta = 0.2f;

    private void Update()
    {
        // Is this for a cooldown on placing tiles? If so, my Timer class would help make this easier B)
        // I'll put it into the project just in case - Brad

        //Yes, this is a timer. Go ahead and change it if you have a better method. - Kevin
        if (!active)
        {
            delta = 1;
        }
        else
        {
            delta -= Time.deltaTime;
        }

        if (active && delta < 0)
        {
            active = false;
        }

        if (isLeftClicking() && !active)
        {
            PlaceTileAtCell(MousePositionToCellPosition.CellHovered);
            active = true;
        }
    }
}
