using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public float scrollSpeed = 0.1f; // Speed of the background scrolling
    public float backgroundHeight; // Height of the background sprite
    public int loopCount = 3; // Number of times to loop the background

    private Vector3 startPosition;
    private int currentLoop = 0; // Track the number of completed loops

    void Start()
    {
        startPosition = transform.position;

        if (backgroundHeight == 0)
        {
            Debug.LogError("Background height is not set. Please set the background height in the inspector.");
        }
    }

    void Update()
    {
        if (currentLoop < loopCount)
        {
            float newPosition = Mathf.Repeat(Time.time * scrollSpeed, backgroundHeight);
            transform.position = startPosition + Vector3.down * newPosition;

            // Check if a full loop has completed
            if (Mathf.Abs(newPosition) < 0.01f)
            {
                currentLoop++;
            }
        }
        else
        {
            // Stop the background from scrolling after the specified number of loops
            transform.position = startPosition - Vector3.down * backgroundHeight * loopCount;
        }
    }
}
