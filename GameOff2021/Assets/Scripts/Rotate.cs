using UnityEngine;

// Reference: https://forum.unity.com/threads/simple-rotation-script-free.510303/

public class Rotate : MonoBehaviour
{
    [Range(-1.0f, 1.0f)]
    public float xForceDirection = 0.0f;
    [Range(-1.0f, 1.0f)]
    public float yForceDirection = 0.0f;
    [Range(-1.0f, 1.0f)]
    public float zForceDirection = 0.0f;

    public float speedMultiplier = 1;

    public bool worldPivot = false;

    private Space spacePivot = Space.Self;

    void Start()
    {

        if (worldPivot) spacePivot = Space.World;
    }

    void Update()
    {
        float relativeSpeed = speedMultiplier * Time.deltaTime;
        this.transform.Rotate(xForceDirection * relativeSpeed
                            , yForceDirection * relativeSpeed
                            , zForceDirection * relativeSpeed
                            , spacePivot);
    }

}