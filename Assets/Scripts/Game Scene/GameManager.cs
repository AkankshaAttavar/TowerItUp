using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Add this line to use TextMeshPro

public class GameManager : MonoBehaviour
{
    public GameObject blockPrefab;
    public Transform hook;
    public GameObject platform; // Reference to the platform GameObject
    public float blockHeight = 1.0f;
    public float hookOffset = 3.0f; // Initial offset of the hook
    public float hookMoveValue = 1.0f; // Value by which the hook moves up or down
    public bool firstBlockDropped = false; // Track if the first block has been dropped
    public int maxCollisionsAllowed = 3; // Number of allowed collisions before game stops
    public float collisionTimeWindow = 1f; // Time window to consider multiple collisions as one event
    public float minDistance = 2.0f; // Minimum distance between the hook and top block
    public float maxDistance = 5.0f; // Maximum distance between the hook and top block
    public int alignmentBonusPoints = 100; // Points awarded for perfectly aligned blocks
    public float alignmentMargin = 0.1f; // Margin of error for perfect alignment

    private int blockCount = 0; // Keep track of the number of spawned blocks
    private int collisionEventCount = 0; // Keep track of the number of collision events
    private float lastCollisionTime = 0; // Track the time of the last collision event
    private GameObject currentBlock;
    private GameObject previousBlock;
    public List<GameObject> stackedBlocks = new List<GameObject>(); // Track stacked blocks that moved the hook up

    // Timer variables
    private bool timerStarted = false;
    private float timer = 0f;
    public float gameDuration = 60f; // Game duration in seconds (1 minute)

    // UI references
    public TextMeshProUGUI timerText; // Use TextMeshProUGUI for UI text
    public TextMeshProUGUI timesUpText; // Use TextMeshProUGUI for UI text
    public TextMeshProUGUI scoreText; // Use TextMeshProUGUI for score text

    // Score variable
    private int score = 0;

    private CameraController cameraController; // Reference to the CameraController

    // Track position of the last placed block
    public Vector3 LastPlacedBlockPosition { get; private set; }

    void Start()
    {
        SetInitialHookPosition();
        SpawnNewBlock();
        cameraController = FindObjectOfType<CameraController>();
        timesUpText.gameObject.SetActive(false); // Ensure "Time's Up" text is hidden initially
        UpdateScoreText(); // Initialize score text
    }

    void Update()
    {
        // Start the timer when space is pressed for the first time
        if (!timerStarted && Input.GetKeyDown(KeyCode.Space))
        {
            timerStarted = true;
        }

        // Update the timer if it has started
        if (timerStarted)
        {
            timer += Time.deltaTime;
            float timeRemaining = gameDuration - timer;
            if (timeRemaining > 0)
            {
                timerText.text = "Time: " + timeRemaining.ToString("F2"); // Display remaining time
            }
            else
            {
                timerText.text = "Time: 0.00";
                timesUpText.gameObject.SetActive(true); // Display "Time's Up" text
                StartCoroutine(EndGame());
            }
        }
    }

    private void SetInitialHookPosition()
    {
        Vector3 initialPosition = new Vector3(hook.position.x, hookOffset, hook.position.z);
        hook.position = initialPosition;
        Debug.Log("Initial Hook Position: " + hook.position.y);
    }

    public void SpawnNewBlock()
    {
        if (currentBlock != null)
        {
            previousBlock = currentBlock;
        }

        // Adjust the spawn position to reduce the gap between the hook and the new block
        Vector3 spawnPosition = new Vector3(hook.position.x, hook.position.y - blockHeight / 4, hook.position.z);
        currentBlock = Instantiate(blockPrefab, spawnPosition, Quaternion.identity);
        currentBlock.tag = "Block";
        hook.GetComponent<Hook>().AttachBlock(currentBlock.transform);

        // Increase hook radius and speed after every 10 blocks spawned
        blockCount++;
        if (blockCount % 10 == 0)
        {
            Hook hookScript = hook.GetComponent<Hook>();
            hookScript.IncreaseRadiusAndSpeed(0.8f, 0.1f);
        }
    }

    public void StartSpawnNewBlockRoutine()
    {
        StartCoroutine(SpawnNewBlockAfterDelay(1f));
    }

    private IEnumerator SpawnNewBlockAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnNewBlock();
    }

    public void OnBlockAttached(GameObject block)
    {
        stackedBlocks.Add(block); // Track this block as it moved the hook up
        if (block != null)
        {
            LastPlacedBlockPosition = block.transform.position; // Update the position of the last placed block
        }

        CheckAlignment(block);

        StartCoroutine(AdjustHookPositionWithDelay());

        // Increase score when a block is attached
        score += 10; // Adjust the score increment as needed
        UpdateScoreText();
    }

    private void CheckAlignment(GameObject block)
    {
        if (stackedBlocks.Count > 1)
        {
            GameObject previousBlock = stackedBlocks[stackedBlocks.Count - 2];
            if (Mathf.Abs(block.transform.position.x - previousBlock.transform.position.x) <= alignmentMargin)
            {
                // Award bonus points for perfect alignment
                AwardAlignmentBonus();
            }
        }
    }

    private void AwardAlignmentBonus()
    {
        // Award points for perfect alignment
        Debug.Log("Perfect Alignment! Awarded " + alignmentBonusPoints + " points.");
        score += alignmentBonusPoints; // Add bonus points to score
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + score; // Update score text
    }

    public void OnBlockDestroyed(GameObject block)
    {
        // Only adjust hook position if this block was previously counted
        if (stackedBlocks.Contains(block))
        {
            stackedBlocks.Remove(block);
            StartCoroutine(AdjustHookPositionWithDelay());
        }
    }

    public void OnBlockFallen(GameObject block)
    {
        float currentTime = Time.time;

        if (currentTime - lastCollisionTime > collisionTimeWindow)
        {
            collisionEventCount++; // Increment collision event count
            lastCollisionTime = currentTime; // Update last collision time
            CheckGameOver(); // Check if the game should stop
        }

        // Destroy the block if it hits the ground
        stackedBlocks.Remove(block);
        Destroy(block);

        // Deduct score when a block falls
        score -= 100; // Adjust the score deduction as needed
        UpdateScoreText();
    }

    private void CheckGameOver()
    {
        if (collisionEventCount >= maxCollisionsAllowed)
        {
            Debug.Log("Game Over!");
            StartCoroutine(EndGame()); // Start the coroutine to pan the camera
        }
    }

    private IEnumerator EndGame()
    {
        // Pan the camera to the platform
        if (cameraController != null && platform != null)
        {
            yield return cameraController.PanToPosition(platform.transform.position);
        }

        // Stop the game (e.g., by reloading the scene or showing a game over screen)
        // Here, we simply reload the current scene for demonstration purposes
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator AdjustHookPositionWithDelay()
    {
        yield return new WaitForSeconds(0.5f); // Introduce a small delay
        AdjustHookPosition();
    }

    private void AdjustHookPosition()
    {
        if (hook == null || stackedBlocks.Count == 0)
        {
            Debug.LogError("Hook transform is null or no blocks are stacked");
            return;
        }

        GameObject topBlock = null;
        float highestY = float.MinValue;

        foreach (var block in stackedBlocks)
        {
            if (block != null && block.transform.position.y > highestY)
            {
                topBlock = block;
                highestY = block.transform.position.y;
            }
        }

        if (topBlock != null)
        {
            float distance = hook.position.y - topBlock.transform.position.y;

            if (distance < minDistance)
            {
                float newYPosition = topBlock.transform.position.y + minDistance;
                hook.position = new Vector3(hook.position.x, newYPosition, hook.position.z);
            }
            else if (distance > maxDistance)
            {
                float newYPosition = topBlock.transform.position.y + maxDistance;
                hook.position = new Vector3(hook.position.x, newYPosition, hook.position.z);
            }

            hook.GetComponent<Hook>().AdjustHookPosition(hook.position);

            Debug.Log("Adjusted Hook Position: " + hook.position.y);
        }
    }
}
