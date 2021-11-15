using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class LevelEditor : MonoBehaviour
{
    public Level Level;
    public Tilemap Tilemap;
    public GameObject PiecePrefab;
    public Transform TilemapParent;
    public List<GameObject> Pieces;

    public void AddPiece(GameObject GO) => Pieces.Add(GO);

    // TODO Pieces on changes?
    private void Update()
    {
        //Debug.Log("Something happened");
    }
}
