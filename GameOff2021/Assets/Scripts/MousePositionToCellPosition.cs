using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Grid))]
public class MousePositionToCellPosition : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] bool debug;
    Grid grid;
    public static Vector3Int CellHovered;

    // Start is called before the first frame update
    void Awake()
    {
        if (cam == null) cam = Camera.main;
        grid = GetComponent<Grid>();
    }

    // Update is called once per frame
    void Update()
    {
        CellHovered = GetCellAtMousePosition();

    }

    private void FixedUpdate()
    {
        if (debug) Debug.Log(CellHovered);
    }

    Vector3Int GetCellAtMousePosition()
    {
        Vector3Int cell = grid.WorldToCell(cam.ScreenToWorldPoint(Input.mousePosition));
        cell.z = 0;

        return cell;
    }
}
