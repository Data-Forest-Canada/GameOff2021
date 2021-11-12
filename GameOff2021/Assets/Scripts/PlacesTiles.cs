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
        Tile atLocation = tilemap.GetTile<Tile>(cell);
        Tile newTile = atLocation?.CombineWith(toPlace);
        tilemap.SetTile(cell, newTile ?? atLocation);
        Debug.Log($"{toPlace} + {atLocation} = {newTile}");
    }

    bool isLeftClicking() => Input.GetMouseButtonDown(0);
    bool active = false;

    float delta = 1;

    private void Update()
    {
        if (!active)
        {
            delta = 1;
        }
        else
        {
            delta -= Time.deltaTime;
        }

        if(active && delta < 0)
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
