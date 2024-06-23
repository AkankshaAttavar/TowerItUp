using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBlock : MonoBehaviour
{
    private Rigidbody2D rb;
    public bool isReleased = false;
    private GameManager gameManager;
    private Hook hook;
    private BoxCollider2D boxCollider;
    private AudioSource audioSource;
    private float soundCooldown = 1.0f; // Cooldown duration in seconds
    private float lastSoundTime; // Time when the sound was last played

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component

        rb.gravityScale = 1;
        rb.mass = 1;
        rb.drag = 2;
        rb.angularDrag = 3f;

        gameManager = FindObjectOfType<GameManager>();
        hook = FindObjectOfType<Hook>();

        if (gameManager == null)
        {
            Debug.LogError("GameManager not found!");
        }

        if (hook == null)
        {
            Debug.LogError("Hook not found!");
        }
        else
        {
            hook.AttachBlock(transform);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isReleased)
        {
            ReleaseBlock();
            if (gameManager != null)
            {
                gameManager.StartSpawnNewBlockRoutine();
            }
        }
    }

    void ReleaseBlock()
    {
        isReleased = true;
        rb.gravityScale = 1;
        if (hook != null)
        {
            hook.ReleaseBlock();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isReleased)
        {
            if (collision.gameObject.CompareTag("Block") || collision.gameObject.CompareTag("Ground"))
            {
                // Play collision sound only if colliding with Block or Ground and cooldown has passed
                if (audioSource != null && Time.time - lastSoundTime > soundCooldown)
                {
                    audioSource.Play();
                    lastSoundTime = Time.time;
                }

                if (collision.gameObject.CompareTag("Block"))
                {
                    if (gameManager != null)
                    {
                        // Freeze blocks if alignment is within margin
                        if (Mathf.Abs(transform.position.x - collision.transform.position.x) <= gameManager.alignmentMargin)
                        {
                            FreezeBlockAndBelow(collision.gameObject);
                        }

                        gameManager.OnBlockAttached(this.gameObject);
                    }
                }
                else if (collision.gameObject.CompareTag("Ground"))
                {
                    if (gameManager != null)
                    {
                        gameManager.OnBlockFallen(this.gameObject); // Notify GameManager about the fallen block
                    }
                    Destroy(gameObject); // Destroy the block when it hits the ground
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (gameManager != null)
            {
                gameManager.OnBlockFallen(this.gameObject); // Notify GameManager about the fallen block
            }
        }
    }

    public void FreezeBlockAndBelow(GameObject otherBlock)
    {
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
        transform.parent = gameManager.transform;

        // Freeze the block below if it has alignment margin below 0.05f
        if (gameManager.stackedBlocks.Count > 1)
        {
            GameObject previousBlock = gameManager.stackedBlocks[gameManager.stackedBlocks.Count - 2];
            if (Mathf.Abs(previousBlock.transform.position.x - otherBlock.transform.position.x) <= 0.05f)
            {
                Rigidbody2D previousRb = previousBlock.GetComponent<Rigidbody2D>();
                if (previousRb != null)
                {
                    previousRb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
                }
                previousBlock.transform.parent = gameManager.transform;
            }
        }
    }
}
