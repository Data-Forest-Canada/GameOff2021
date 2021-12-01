using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// BackgroundMovement 
/// Creates a background animation for the main menus by generating puzzle pieces
/// and manipulating their attributes such as velocity to improve menu aesthetics.
///
/// Supports multiple animation patterns.
public class BackgroundAnimation : MonoBehaviour
{
    enum SpawnType
    {
        Horizontal, // Pieces are spawned along the X-axis, and move toward the Destination object.
        Vertical, // Pieces are spawned along the Y-axis, and move toward the Destination object.      
        Radial // Pieces are spawned at a central point and move outward randomly.
    }

    // Piece Generation
    [Header("Setup:")]
    public GameObject Destination;
    public BackgroundSprite BackgroundSpritePrefab;
    public Sprite[] Sprites;
    public Color[] Colours;

    // Piece Settings    
    [Header("Piece Settings:")]


    public float minSize = 0.05f;
    public float maxSize = 0.1f; // 
    [Range(1f, 5f)]
    public float speedScaleModifier = 2f; // The amount we change the speed based on the size of the piece.
    [Range(1f, 100f)]
    public float movementSpeed = 5f;
    [Range(0f, 100f)]
    public float rotationSpeed = 10f;
    public bool IsRandomRotation = true;

    // Spawn Settings
    [Header("Spawner Settings:")]
    public int maxPieces = 100;
    [Range(0f, 1)]
    public float spawnDelay = 0f;

    [Range(0f, 500f)]
    public float verticalSpawnRangeSize = 200f;

    [Range(0f, 1920f)]
    public float horizontalSpawnRangeSize = 200f;

    // TODO: I want to make a dropdown for the spawn types in the editor, but I don't know how to do custom editors.        
    [Range(0, 2)]
    public int spawnType = (int)SpawnType.Vertical;

    // Private Variables
    public Camera cam;
    Vector2 direction;
    List<BackgroundSprite> activeSprites;
    List<BackgroundSprite> inactiveSprites;

    bool isSpawning = false;

    /// Start is called before the first frame update.   
    void Start()
    {
        if (cam == null)
            cam = Camera.main;

        // Instantiate the list of puzzle pieces.
        activeSprites = new List<BackgroundSprite>();
        inactiveSprites = new List<BackgroundSprite>();

        // Pregenerate teh pieces and sizes
        for (int i = 0; i < maxPieces; i++)
        {
            BackgroundSprite newPiece = spawnPiece();

            // Pick a size based on the current piece.
            // This will result in larger pieces appearing first in the heirarchy.
            // TODO: this could be a place to use a design curve. Allow us to chose the curve of values between the two points.
            Vector3 newSize = Vector3.Slerp(new Vector3(minSize, minSize, 1), new Vector3(maxSize, maxSize, 1), (float)i / (maxPieces - 1));
            newPiece.SetSize(newSize);

            inactiveSprites.Add(newPiece);
        }

        // Begins spawning the pieces.
        IEnumerator Coroutine = CoActivatePieces(spawnDelay);
        StartCoroutine(Coroutine);
    }

    /// Update is called once per frame.
    void Update()
    {
        // We can restart the spawner if the max number of pieces changes during runtime.
        // But we have to make sure the coroutine isn't currently spawning pieces, otherwise
        // multiple coroutines will spawn pieces and reach the limit nearly instantly.
        if (activeSprites.Count < maxPieces)
        {
            if (!isSpawning)
            {
                IEnumerator Coroutine = CoActivatePieces(spawnDelay);
                StartCoroutine(Coroutine);
            }
        }

        // Check each puzzle piece to see if it has passed the target destination so we can 
        // reset it, update the attributes if they were changed in the editor, and randomize
        // attributes such as the colour or shape of the piece.
        foreach (BackgroundSprite piece in activeSprites)
        {
            // The target destination is based on the Finish GameObject in the MainMenu scene.
            if (PieceReachedTargetDestination(piece))
            {
                // Reset the position to a new spawn and check if any attributes
                // have been modified in the editor during runtime.
                activatePiece(piece);
            }
        }
    }

    /// Spawn new puzzle pieces at the start position until the maximum number of pieces is reached.    
    private IEnumerator CoActivatePieces(float spawnDelay)
    {
        // The puzzle piece is only spawned if the maximum number of pieces has not been reached.
        while (inactiveSprites.Count > 0)
        {
            // This is used to check if the coroutine is already running in Update()        
            isSpawning = true;

            // Create a puzzle piece with a random starting rotation.
            BackgroundSprite inactivePiece = getRandomInactivePiece();

            activatePiece(inactivePiece);

            // Add the piece to the list which we check 
            activeSprites.Add(inactivePiece);

            yield return new WaitForSeconds(spawnDelay);
        }

        // When the coroutine reaches the maximum number of pieces, we need to track that it has completed.
        // Then if the maximum number of pieces is modified at runtime we can restart it in Update().
        isSpawning = false;
    }

    BackgroundSprite spawnPiece()
    {
        // Create a puzzle piece with a random starting rotation.
        BackgroundSprite newPiece = Instantiate(
            BackgroundSpritePrefab,
            Vector2.zero,
            Quaternion.Euler(0, 0, Random.Range(-180.0f, 180.0f)
        ));

        // Set the parent to the BackgroundMovement GameObject. This will set its position to (0, 0, 0) in the parent object.            
        newPiece.transform.SetParent(transform, false);

        return newPiece;
    }

    // Gets a random piece from the inactive piece list, null if empty
    BackgroundSprite getRandomInactivePiece()
    {
        // Bounds check
        if (inactiveSprites.Count > 0)
        {
            // Pick a piece and return it
            int rand = Random.Range(0, inactiveSprites.Count - 1);

            return inactiveSprites[rand];
        }

        return null;
    }

    /// The attributes of the puzzle piece get updated in only two cases,
    /// on instantiation and when it reaches its target destination and needs to reset.
    /// This allows us to modify the attributes in the editor during runtime and have pieces update.
    void activatePiece(BackgroundSprite puzzlePiece)
    {
        // POSITION
        puzzlePiece.SetPosition(GetRandomSpawnLocation());

        // DIRECTION

        // Determines the direction the pieces will move based on the Start and Finish GameObjects in the scene.
        //CalculateMovementDirection();

        // Radial spawning requires a new random position for each piece.
        if (spawnType == (int)SpawnType.Radial)
        {
            //GetRandomDirection();
            //puzzlePiece.transform.position += puzzlePiece.transform.forward;
        }

        UnityEngine.Debug.Log(puzzlePiece.transform.up);
        UnityEngine.Debug.Log(movementSpeed);
        // MOVEMENT SPEED
        puzzlePiece.SetMovementVelocity(puzzlePiece.transform.up * movementSpeed);

        // ROTATION SPEED
        float areaOfPuzzlePiece = puzzlePiece.transform.localScale.x * puzzlePiece.transform.localScale.y;
        //puzzlePiece.SetRotationVelocity(GetSpeedRelativeToSize(rotationSpeed, areaOfPuzzlePiece));

        // ROTATION DIRECTION
        // 50/50 chance to reverse the direction.
        if (IsRandomRotation)
        {
            if (Random.Range(0, 2) == 0)
            {
                puzzlePiece.ReverseRotationDirection();
            }
        }

        // COLOR
        puzzlePiece.SetColor(GetRandomPieceColour());

        // SHAPE
        puzzlePiece.SetImage(GetRandomPieceShape());

        // Remove from list
        inactiveSprites.Remove(puzzlePiece);
    }

    /// Calculates the direction the pieces should move using the position of the Start and Finish GameObjects.
    void CalculateMovementDirection()
    {
        direction = new Vector2(
            Destination.transform.position.x - transform.position.x,
            Destination.transform.position.y - transform.position.y
        );

        // The magnitude of the vector is irrelevant, so we normalize it to a length of 1.
        direction.Normalize();
    }

    /// Randomly generates a direction in 360 degrees.
    /// Used when the spawn type is radial.
    void GetRandomDirection()
    {
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);

        direction = new Vector2(x, y);
        direction.Normalize();
    }

    /// Generates a random spawn location, random either on the x-axis, y-axis, or neither axis,
    /// depending on the spawn type.
    Vector3 GetRandomSpawnLocation()
    {
        Vector3 spawnLocation = Vector3.zero;

        switch (spawnType)
        {
            // Spawn with a random position on the X-axis.
            case (int)SpawnType.Horizontal:
                spawnLocation = new Vector3(transform.position.x + Random.Range(-horizontalSpawnRangeSize, horizontalSpawnRangeSize), transform.position.y, transform.position.z);
                break;

            // Spawn with a random position on the X-axis.
            case (int)SpawnType.Vertical:
                spawnLocation = new Vector3(transform.position.x, transform.position.y + Random.Range(-verticalSpawnRangeSize, verticalSpawnRangeSize), transform.position.z);
                break;

            // Spawn at the position of the BackgroundMovement object.
            case (int)SpawnType.Radial:
                spawnLocation = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                break;

            default:
                UnityEngine.Debug.Log("Invalid Spawn Type: Blame Matthew for allowing this to happen.");
                break;
        }

        return spawnLocation;
    }

    /// Checks if the puzzle piece has reached the target destination based on what spawn type is selected.
    bool PieceReachedTargetDestination(BackgroundSprite piece)
    {
        // TODO: This currently assumes that the direction to the target destination is in the positive x or y values.
        //       eg. If our spawner is at (0,0) it will not reset objects if the destination is placed at (0, -10).
        //       It should check the direction and adjust the condition.

        switch (spawnType)
        {
            // The piece should not pass the destination on the y-axis.
            case (int)SpawnType.Horizontal:
                if (transform.position.y < Destination.transform.position.y)
                {

                    if (piece.GetPosition().y >= Destination.transform.position.y)
                    {
                        return true;
                    }
                }
                else
                {
                    if (piece.GetPosition().y <= Destination.transform.position.y)
                    {
                        return true;
                    }

                }

                break;

            // The piece should not pass the destination on the x-axis.
            case (int)SpawnType.Vertical:
                if (transform.position.x < Destination.transform.position.x)
                {

                    if (piece.GetPosition().x >= Destination.transform.position.x)
                    {
                        return true;
                    }
                }
                else
                {

                    if (piece.GetPosition().x <= Destination.transform.position.x)
                    {
                        return true;
                    }
                }
                break;

            // The piece should be reset when it goes too far off screen.
            case (int)SpawnType.Radial:
                if (PieceIsOffscreen(piece))
                {
                    return true;
                }
                break;

            default:
                return false;
        }

        return false;
    }

    // Determines whether or not a BackgroundPiece is still visible on the screen.
    bool PieceIsOffscreen(BackgroundSprite piece)
    {
        if (!piece.GetComponent<Renderer>().isVisible)
        {
            return true;
        }

        return false;

        // Get the position of the piece relative to the camera.
        Vector3 currentPosition = cam.WorldToViewportPoint(piece.GetPosition());

        // TODO: This value is not relative to the camera and as a result
        //       the method takes longer to return true than necessary.
        //       But it's close enough.
        Vector3 pieceSize = piece.GetScale();

        // "Inside" the viewport would be any (x, y) values between 0 and 1, so we check if the piece 
        // is outside that range. But since the origin of the piece is centered, the size of the piece
        // must also be considered. They should be outside of the viewport by at least their own size,
        // to prevent pieces from disappearing while halfway off the screen.    
        if (currentPosition.x + pieceSize.x < 0 || // Left side of the screen.
            currentPosition.x - pieceSize.x > 1 || // Right side of the screen.
            currentPosition.y + pieceSize.y < 0 || // Bottom of the screen.
            currentPosition.y - pieceSize.y > 1)   // Top of the screen.
        {
            return true;
        }

        return false;
    }

    // Returns a random piece size based on the min and max values in the editor.
    Vector3 GetRandomPieceSize()
    {
        float randomSize = Random.Range(minSize, maxSize);
        return new Vector3(randomSize, randomSize, 0);
    }

    // Returns a random colour based on the colours provided to the array in the editor.
    Color GetRandomPieceColour()
    {
        return Colours[Random.Range(0, Colours.Length)];
    }

    Sprite GetRandomPieceShape()
    {
        return Sprites[Random.Range(0, Sprites.Length)];
    }

    // Returns the speed a piece should be moving based on its size.
    // This is because the size represents the distance from the camera.
    // Larger pieces are closer and should therefore appear to move faster.
    Vector3 GetSpeedRelativeToSize(float speed, Vector3 pieceSize)
    {
        Vector3 newSpeed = new Vector3(pieceSize.x * speedScaleModifier * speed, pieceSize.y * speedScaleModifier * speed, pieceSize.z);
        return newSpeed;
    }

    // Returns the speed a piece should rotate at based on its size.
    // This is because the size represents the distance from the camera.
    // Larger objects are closer to the camera and should appear to rotate faster.
    float GetSpeedRelativeToSize(float speed, float pieceSize)
    {
        return speed * (pieceSize * speedScaleModifier);
    }

    void OnDrawGizmos()
    {
        // Draw indicators for the range where objects will spawn or reset.
        Gizmos.color = new Color(255f / 255f, 188f / 255f, 10f / 255f, 1f);

        // Calculate the direction from the Start object to the Destination (Finish) object.
        Vector3 direction = Destination.transform.position - transform.position;
        Vector3 minTarget;
        Vector3 maxTarget;

        // The boundaries change based on what type of animation/spawn is selected.
        switch (spawnType)
        {
            // Horizontal
            case (int)SpawnType.Horizontal:
                // Calculate the min and max spawn locations based on the horizontal spawn range size.
                minTarget = new Vector3(transform.position.x - horizontalSpawnRangeSize, transform.position.y);
                maxTarget = new Vector3(transform.position.x + horizontalSpawnRangeSize, transform.position.y);

                DrawSpawnBoundaries(minTarget, maxTarget, direction);
                break;

            // Vertical
            case (int)SpawnType.Vertical:
                // Calculate the min and max spawn locations based on the vertical spawn range size.
                minTarget = new Vector3(transform.position.x, transform.position.y - verticalSpawnRangeSize);
                maxTarget = new Vector3(transform.position.x, transform.position.y + verticalSpawnRangeSize);

                DrawSpawnBoundaries(minTarget, maxTarget, direction);
                break;

            // Radial
            case (int)SpawnType.Radial:
                break;
            default:
                break;
        }
    }

    // Draws lines and spheres to show the boundaries the pieces will spawn and move in.
    void DrawSpawnBoundaries(Vector3 minTarget, Vector3 maxTarget, Vector3 targetDirection)
    {
        Vector3 minBoundary = minTarget + targetDirection;
        Vector3 maxBoundary = maxTarget + targetDirection;

        //   1. |             +             | 5.
        //      |                           |
        //      |                           |
        //   2. +-------------+-------------+ 4.
        //                    3.
        Gizmos.DrawLine(minTarget, minBoundary);    // 1 
        Gizmos.DrawSphere(minTarget, 2);            // 2
        Gizmos.DrawLine(minTarget, maxTarget);      // 3
        Gizmos.DrawSphere(maxTarget, 2);            // 4
        Gizmos.DrawLine(maxTarget, maxBoundary);    // 5
    }
}
