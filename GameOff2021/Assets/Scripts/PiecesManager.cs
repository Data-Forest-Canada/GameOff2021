using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PiecesManager : MonoBehaviour
{
    // Inspector variables and their static copies, we need these to support static methods
    [SerializeField] GameObject piecePrefab;

    [SerializeField] Transform piecesParent;

    // Single list of pieces we store
    List<GameObject> pieces;

    // This should be a singleton, ensuring only it exists
    static PiecesManager instance;

    public static PiecesManager Instance { get { return instance; } }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            pieces = new List<GameObject>();
        }
        else
        {
            print("A PieceManager Already exists");
            Destroy(gameObject);
        }
    }

    // We might want this class to handle finding the position of the pieces as well.
    // If so we can remove the position parameter
    public void AddPiece(MultiTile piece, Vector3 position)
    {
        // Create new piece and apply the tilemap
        GameObject newPiece = Instantiate(piecePrefab, position, Quaternion.identity);

        Tilemap pieceTilemap = newPiece.GetComponent<Tilemap>();
        pieceTilemap.ApplyMultiTileAt(piece, Vector3Int.zero);

        // Set the parent and add to the list
        newPiece.transform.SetParent(piecesParent);

        pieces.Add(newPiece);
    }

    public void ResetPieces()
    {
        foreach (GameObject piece in pieces)
        {
            Destroy(piece);
        }

        pieces.Clear();
    }
}
