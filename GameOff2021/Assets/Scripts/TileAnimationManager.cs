using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class TileAnimationManager : MonoBehaviour
{
    Tilemap map;
    Timer startTimer, stopTimer;
    Queue<Vector3Int> animatedPositions;

    private void Awake()
    {
        animatedPositions = new Queue<Vector3Int>();
        map = GetComponent<Tilemap>();

        startTimer = new Timer(this, 1.5f);
        startTimer.OnTimerCompleted += AnimateTile;

        stopTimer = new Timer(this, 0.5f);
        stopTimer.OnTimerCompleted += StopAnimation;
    }

    public void AnimateTile()
    {
        BoundsInt bounds = map.cellBounds;
        int x = Random.Range(bounds.xMin, bounds.xMax);
        int y = Random.Range(bounds.yMin, bounds.yMax);
        int z = Random.Range(bounds.zMin, bounds.zMax);

        Vector3Int posToAnimate = new Vector3Int(x, y, z);

        GameTile tile = map.GetTile<GameTile>(posToAnimate);

        if (tile != null)
        {
            animatedPositions.Enqueue(posToAnimate);
            if (!stopTimer.IsActive)
                stopTimer.Start();
        }

        map.SetTile(posToAnimate, tile?.MatchingTile);

        //startTimer.OnTimerCompleted += AnimateTile;
        startTimer.Restart();
    }

    public void StopAnimation()
    {
        Vector3Int posToStop = animatedPositions.Dequeue();

        GameTile tile = map.GetTile<GameTile>(posToStop);
        // Just dodging an exception for launch
        if (tile == null) return;
        map.SetTile(posToStop, tile.MatchingTile);

        if (animatedPositions.Count > 0)
            stopTimer.Start();

    }

    // Start is called before the first frame update
    void Start()
    {
        startTimer.Start();
    }

    public void LevelCompleteHandler()
    {
        BoundsInt.PositionEnumerator enu = map.cellBounds.allPositionsWithin;
        GameTile tile;
        while (enu.MoveNext())
        {
            tile = map.GetTile<GameTile>(enu.Current);
            if (tile != null)
            {
                map.SetTile(enu.Current, tile.MatchingTile);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
