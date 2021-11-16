using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class GridEditor : MonoBehaviour
{
    [SerializeField] Tilemap tileMap;
    [SerializeField] GameTile defaultTile;

    public static GameTile SelectedTile;

    private void Awake()
    {
        SelectedTile = defaultTile;
    }

    private void Update()
    {
        if (isLeftClicking() && SelectedTile != null)
        {
            setHoveredTileTo(SelectedTile);
            //setHoveredTileTo(SelectedTile.TileBase);
        }
        else if (isRightClicking())
        {
            SelectedTile = defaultTile;
        }
    }

    bool isLeftClicking() => Input.GetMouseButtonDown(0);
    bool isRightClicking() => Input.GetMouseButtonDown(1);

    void setHoveredTileTo(TileBase tileBase)
    {
        Vector3Int cellHovered = MousePositionToCellPosition.CellHovered;
        tileMap.SetTile(cellHovered, tileBase);
    }

    public void ChangeSelectedTile(GameTile tile)
    {
        SelectedTile = tile;
    }
}
