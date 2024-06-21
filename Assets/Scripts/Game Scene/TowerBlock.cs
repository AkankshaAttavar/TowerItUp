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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        rb.gravityScale = 0;
        rb.mass = 5;
        rb.drag = 0;
        rb.angularDrag = 0.05f;

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
        if (Input.GetKeyDown(KeyCode.Space) && !isReleased)
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
            if (collision.gameObject.CompareTag("Block"))
            {
                if (IsAlignedWithBlock(collision.gameObject))
                {
                    // Make the blocks stick together
                    FreezeBlockAndBelow();
                    TowerBlock otherBlock = collision.gameObject.GetComponent<TowerBlock>();
                    if (otherBlock != null)
                    {
                        otherBlock.FreezeBlockAndBelow();
                    }
                }
                if (gameManager != null)
                {
                    gameManager.OnBlockAttached(this.gameObject);
                }
            }
            else if (collision.gameObject.CompareTag("Ground"))
            {
                if (gameManager != null)
                {
                    gameManager.OnBlockFallen(this.gameObject); // Notify GameManager about the fallen block
                }
            }
        }
    }

    private bool IsAlignedWithBlock(GameObject otherBlock)
    {
        float alignmentMargin = 0.1f; // 10% margin of error
        float offsetX = Mathf.Abs(transform.position.x - otherBlock.transform.position.x);
        return offsetX <= alignmentMargin * boxCollider.size.x;
    }

    public void FreezeBlockAndBelow()
    {
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
        if (gameManager != null)
        {
            transform.parent = gameManager.transform;

            // Freeze all blocks below
            foreach (var block in gameManager.stackedBlocks)
            {
                if (block != null && block.transform.position.y <= transform.position.y)
                {
                    Rigidbody2D blockRb = block.GetComponent<Rigidbody2D>();
                    if (blockRb != null)
                    {
                        blockRb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
                    }
                    block.transform.parent = gameManager.transform;
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
}
