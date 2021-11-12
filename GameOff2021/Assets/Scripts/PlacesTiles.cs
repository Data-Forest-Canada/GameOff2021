using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlacesTiles : MonoBehaviour
{
    public Tile toPlace;
    public Tilemap tilemap;

    public void ChangeTileToPlace(Tile tile)
    {
        toPlace = tile;
    }

    public void PlaceTileAtCell(Vector3Int cell)
    {
        //Debug.Log(tilemap.GetTile<Tile>(cell));
        Debug.Log(toPlace);
        Tile atLocation = tilemap.GetTile<Tile>(cell);
        Tile newTile = atLocation?.CombineWith(toPlace) ?? atLocation;
        tilemap.SetTile(cell, newTile);
        //Debug.Log($"{toPlace} + {atLocation} = {newTile}");
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
