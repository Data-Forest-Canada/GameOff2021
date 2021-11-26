using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "GameTile/GetPiece")]
public class GetPieceGameTile : GameTile
{
    [SerializeField] MultiTile tileToGet;
    [SerializeField] MultiTile[] bucketTiles;
    [SerializeField] bool useBucket;

    Bucket<MultiTile> bucket;

    private void Awake()
    {
        bucket = new Bucket<MultiTile>(bucketTiles);
    }

    public override void OnPlace(Tilemap map, Vector3Int position)
    {
        if (useBucket)
        {
            PiecesManager.Instance.AddPiece(bucket.GetNextItem(), Vector3.zero);
        }
        else
        {
            PiecesManager.Instance.AddPiece(tileToGet, Vector3.zero);
        }
    }
}
