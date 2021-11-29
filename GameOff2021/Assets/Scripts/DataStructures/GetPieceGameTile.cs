using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "GameTile/GetPiece")]
public class GetPieceGameTile : GameTile
{
    [SerializeField] MultiTile tileToGet;
    [SerializeField] bool useBucket;
    [SerializeField] MultiTile[] bucketTiles;

    Bucket<MultiTile> bucket;

    public override void OnPlace(Tilemap map, Vector3Int position)
    {
        base.OnPlace(map, position);

        if (useBucket)
        {
            if (bucket == null) bucket = new Bucket<MultiTile>(bucketTiles);
            PiecesManager.Instance.AddPiece(bucket.GetNextItem(), Vector3.zero);
        }
        else
        {
            PiecesManager.Instance.AddPiece(tileToGet, Vector3.zero);
        }
    }
}
