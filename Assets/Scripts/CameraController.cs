using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera mainCamera;
    public float blockHeight = 2.0f; // Height of a single block
    private float highestBlockY = 0.0f;
    private int blockCount = 0; // Counter to track number of blocks placed

    public void OnBlockStacked(float blockTopY)
    {
        blockCount++;
        highestBlockY = blockTopY;
        if (blockCount > 1) // Start moving camera after the second block is placed
        {
            MoveCameraUp(highestBlockY + blockHeight * 3); // Move camera up 3 block heights above
        }
    }

    public void MoveCameraUp(float yPosition)
    {
        Vector3 newCameraPosition = mainCamera.transform.position;
        newCameraPosition.y = yPosition;
        mainCamera.transform.position = newCameraPosition;
    }
}
