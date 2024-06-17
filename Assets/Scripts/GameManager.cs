using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject blockPrefab;
    public Transform hook;
    public Camera mainCamera;

    private GameObject currentBlock;
    private GameObject previousBlock;
    public float blockHeight = 1.4f; // Height of a single block
    private float highestBlockY = 0.0f;
    private int blockCount = 0; // Counter to track number of blocks placed

    void Start()
    {
        SpawnNewBlock();
    }

    public void SpawnNewBlock()
    {
        if (currentBlock != null)
        {
            previousBlock = currentBlock;
        }

        Vector3 spawnPosition = new Vector3(hook.position.x, hook.position.y - blockHeight / 2, hook.position.z);
        currentBlock = Instantiate(blockPrefab, spawnPosition, Quaternion.identity);
        currentBlock.tag = "Block";
        hook.GetComponent<Hook>().AttachBlock(currentBlock.transform);
        Debug.Log("New block spawned.");
    }

    public void StartSpawnNewBlockRoutine()
    {
        StartCoroutine(SpawnNewBlockAfterDelay(2f));
    }

    private IEnumerator SpawnNewBlockAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnNewBlock();
    }

    public void OnBlockCollision(GameObject block)
    {
        if (block == currentBlock)
        {
            blockCount++;
            float blockTopY = block.transform.position.y + block.GetComponent<BoxCollider2D>().size.y / 2;
            if (blockTopY > highestBlockY)
            {
                highestBlockY = blockTopY;
            }
        }
    }

    public void OnGroundCollision(GameObject block)
    {
        // No action needed
    }

    public void OnBlockStacked(float blockTopY)
    {
        blockCount++;
        highestBlockY = blockTopY;
        if (blockCount > 1) // Start moving camera after the second block is placed
        {
            MoveCameraUp(highestBlockY + blockHeight); // Move camera up 3 block heights above
        }
    }

    public void MoveCameraUp(float yPosition)
    {
        Vector3 newCameraPosition = mainCamera.transform.position;
        newCameraPosition.y = yPosition;
        mainCamera.transform.position = newCameraPosition;
    }
}
