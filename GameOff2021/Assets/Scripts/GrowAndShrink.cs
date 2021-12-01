using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Reference: https://answers.unity.com/questions/1323397/how-to-make-an-object-pulse-continuously.html

public class GrowAndShrink : MonoBehaviour
{

    // Grow parameters
    [Range(0.1f, 2f)]
    public float resizeSpeed = 0.02f;

    [Range(1f, 10f)]
    public float maximumSize = 2f;

    [Range(0f, 1f)]
    public float minimumSize = 0.5f;

    private float currentSize = 1;

    // The object being resized.
    private GameObject target;

    private bool isGrowing = true;

    void Awake()
    {
        this.target = this.gameObject;
    }

    void Update()
    {
        float adjustedSpeed = resizeSpeed * Time.deltaTime;
        // The object starts at size 1, which is the lower limit for the maximum size.
        // If the maximum size is set to 1, the object will shrink first. Otherwise it will grow 
        // to the maximum size and then shrink to the minimum size before repeating.
        if (isGrowing)
        {
            // Has not reached the maximum size yet.                        
            if (currentSize < maximumSize)
            {
                // Increase the size of the object based on the resize speed.
                currentSize = Mathf.MoveTowards(currentSize, maximumSize, adjustedSpeed);

                // Update the size of the game object.
                target.transform.localScale = Vector3.one * currentSize;
            }
            else
            {
                // When it reaches maximum size, we switch to shrinking.
                isGrowing = false;
            }
        }
        else
        {
            // Has not reached the minimum size yet.
            if (currentSize > minimumSize)
            {
                // Decrease the size of the object based on the resize speed.
                currentSize = Mathf.MoveTowards(currentSize, minimumSize, adjustedSpeed);

                // Update the size of the game object.
                target.transform.localScale = Vector3.one * currentSize;
            }
            else
            {
                // When it reaches the minimum size, we switch to growing.
                isGrowing = true;
            }
        }
    }
}