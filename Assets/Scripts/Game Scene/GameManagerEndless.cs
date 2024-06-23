using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking; // Add this for UnityWebRequest

public class NewBehaviourScript : MonoBehaviour
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
    public float alignmentMargin = 0.05f; // Margin of error for perfect alignment
    public EndGamePanelController endGamePanelController;
    private int blockCount = 0; // Keep track of the number of spawned blocks

    private int collisionEventCount = 0;
    private float lastCollisionTime = 0;
    private GameObject currentBlock;
    private GameObject previousBlock;
    public List<GameObject> stackedBlocks = new List<GameObject>();

    private bool timerStarted = false;
    private float timer = 0f;
    public float gameDuration = 60f;

    public TextMeshProUGUI timerText;
    public TextMeshProUGUI timesUpText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI finalScoreText;
    public Button shareButton;
    public Button mainMenuButton;
    public Button replayButton;

    private int score = 0;
    private bool isGameOver = false;

    private CameraController cameraController;

    public Vector3 LastPlacedBlockPosition { get; private set; }

    void Start()
    {
        SetInitialHookPosition();
        SpawnNewBlock();
        cameraController = FindObjectOfType<CameraController>();
        timesUpText.gameObject.SetActive(false);
        endGamePanelController.gameObject.SetActive(false);
        shareButton.gameObject.SetActive(false);
        mainMenuButton.gameObject.SetActive(false);
        UpdateScoreText();


    }

    void Update()
    {
        if (!timerStarted && Input.GetMouseButtonDown(0))
        {
            timerStarted = true;
        }

        if (timerStarted)
        {
            timer += Time.deltaTime;
            float timeRemaining = gameDuration - timer;
            if (timeRemaining > 0)
            {
                timerText.text = " " + timeRemaining.ToString("F2");
            }
            else
            {
                timerText.text = "0.00";
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
        if (isGameOver) return;

        if (currentBlock != null)
        {
            previousBlock = currentBlock;
        }

        Vector3 spawnPosition = new Vector3(hook.position.x, hook.position.y - blockHeight / 4, hook.position.z);
        currentBlock = Instantiate(blockPrefab, spawnPosition, Quaternion.identity);
        currentBlock.tag = "Block";
        hook.GetComponent<Hook>().AttachBlock(currentBlock.transform);

        blockCount++;
        if (blockCount % 10 == 0)
        {
            FreezeAllBlocks();
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
        stackedBlocks.Add(block);
        if (block != null)
        {
            LastPlacedBlockPosition = block.transform.position;
        }

        CheckAlignment(block);

        StartCoroutine(AdjustHookPositionWithDelay());

        score += 10;
        UpdateScoreText();
    }

    private void CheckAlignment(GameObject block)
    {
        if (stackedBlocks.Count > 1)
        {
            GameObject previousBlock = stackedBlocks[stackedBlocks.Count - 2];
            if (Mathf.Abs(block.transform.position.x - previousBlock.transform.position.x) <= alignmentMargin)
            {
                AwardAlignmentBonus();
            }
        }
    }

    private void AwardAlignmentBonus()
    {
        Debug.Log("Perfect Alignment! Awarded " + alignmentBonusPoints + " points.");
        score += alignmentBonusPoints;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Score is" + " " + score;
    }

    public void OnBlockDestroyed(GameObject block)
    {
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
            collisionEventCount++;
            lastCollisionTime = currentTime;
            CheckGameOver();
        }

        stackedBlocks.Remove(block);
        Destroy(block);

        score -= 100;
        UpdateScoreText();
    }

    private void CheckGameOver()
    {
        if (collisionEventCount >= maxCollisionsAllowed)
        {
            Debug.Log("Game Over!");
            StartCoroutine(EndGame());
        }
    }

    private IEnumerator EndGame()
    {
        isGameOver = true; // Set the game over state
                           // Pan the camera to the platform
        if (cameraController != null && platform != null)
        {
            yield return cameraController.PanToPosition(platform.transform.position);
        }

        yield return new WaitForSeconds(1f);

        // Fade to black and show "Time's Up" text
        StartCoroutine(FadeToBlack());

        // Show final score and buttons after the fade
        finalScoreText.gameObject.SetActive(true);
        finalScoreText.text = "Final Score: " + score;
        shareButton.gameObject.SetActive(true);
        replayButton.gameObject.SetActive(true);
        mainMenuButton.gameObject.SetActive(true);
        endGamePanelController.gameObject.SetActive(true); // Show end game panel
    }



    private IEnumerator FadeToBlack()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            yield break;
        }

        float fadeDuration = 2f;
        float elapsedTime = 0f;
        Color initialColor = mainCamera.backgroundColor;
        Color targetColor = Color.black;

        while (elapsedTime < fadeDuration)
        {
            mainCamera.backgroundColor = Color.Lerp(initialColor, targetColor, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCamera.backgroundColor = targetColor;
        timesUpText.gameObject.SetActive(false);
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

    private void FreezeAllBlocks()
    {
        foreach (var block in stackedBlocks)
        {
            if (block != null)
            {
                var rb = block.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
                }
            }
        }
    }
}