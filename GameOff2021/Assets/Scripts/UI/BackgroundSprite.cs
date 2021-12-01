using UnityEngine;
using UnityEngine.UI;

/// BackgroundPiece 
/// A background piece is a puzzle piece that is spawned and manipulated by the BackgroundMovement class.
/// This class contains all methods that allow the piece attributes to be changed at runtime.

[RequireComponent(typeof(Rigidbody2D))]
public class BackgroundSprite : MonoBehaviour
{
    SpriteRenderer pieceImage;
    Rigidbody2D pieceRigidbody;

    /// Instantiate variables.
    void Awake()
    {
        pieceImage = GetComponent<SpriteRenderer>();
        pieceRigidbody = GetComponent<Rigidbody2D>();
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public Vector3 GetScale()
    {
        return transform.lossyScale;
    }

    public void SetParent(Transform parent)
    {
        // TODO: Validation could be done here.
        //       Because a background piece should only ever be a child of the BackgroundMovement object.
        transform.parent = parent;
    }

    public void SetColor(Color color)
    {
        //pieceImage.color = color;
    }

    public void SetImage(Sprite sprite)
    {
        pieceImage.sprite = sprite;
    }

    public void SetMovementVelocity(Vector3 velocity)
    {
        // TODO: A minimum and maximum velocity could be implemented here.
        pieceRigidbody.velocity = velocity;
    }

    public void SetRotationVelocity(float velocity)
    {
        // TODO: A minimum and maximum velocity could be implemented here.
        pieceRigidbody.angularVelocity = velocity;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void SetSize(Vector3 size)
    {
        // TODO: A minimum and maximum size could be implemented here.
        transform.localScale = size;
    }

    /// Switches the rotation direction between clockwise and counter-clockwise.    
    public void ReverseRotationDirection()
    {
        pieceRigidbody.angularVelocity *= -1;
    }
}
