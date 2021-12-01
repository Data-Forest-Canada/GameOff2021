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
    public Transform PiecesParent;
    [HideInInspector] public List<GameObject> Pieces;

    private void Update()
    {
        RecalculatePieces();
    }
    public void AddPiece(GameObject GO) => Pieces.Add(GO);

    public void AddPiece()
    {
        GameObject newTilemapObject = Instantiate(PiecePrefab, Vector3.zero, Quaternion.identity, PiecesParent);
        AddPiece(newTilemapObject);
    }


    public void ResetPieces()
    {
        RecalculatePieces();

        foreach (GameObject go in Pieces)
        {
            DestroyImmediate(go);
        }
    }

    public void RecalculatePieces()
    {
        Pieces.Clear();

        foreach (Transform piece in PiecesParent)
        {
            Pieces.Add(piece.gameObject);
        }
    }
}
