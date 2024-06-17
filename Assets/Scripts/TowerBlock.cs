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
    private CameraController cameraController;

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
        cameraController = FindObjectOfType<CameraController>();
        hook.AttachBlock(transform);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isReleased)
        {
            ReleaseBlock();
            gameManager.StartSpawnNewBlockRoutine();
        }
    }

    void ReleaseBlock()
    {
        isReleased = true;
        rb.gravityScale = 1;
        hook.ReleaseBlock();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isReleased)
        {
            if (collision.gameObject.CompareTag("Block") || collision.gameObject.CompareTag("Ground"))
            {
                float overlap = CalculateOverlap(collision.gameObject);

                if (collision.gameObject.CompareTag("Block") && overlap >= 0.8f * boxCollider.size.x)
                {
                    AlignWithPreviousBlock(collision.gameObject);
                    cameraController.OnBlockStacked(transform.position.y + boxCollider.size.y / 2); // Notify CameraController to move camera up
                }

                if (collision.gameObject.CompareTag("Ground") || overlap >= 0.8f * boxCollider.size.x)
                {
                    rb.gravityScale = 0;
                    rb.bodyType = RigidbodyType2D.Static;
                    this.enabled = false;

                    if (gameManager != null)
                    {
                        gameManager.OnBlockCollision(this.gameObject);
                    }

                    Debug.Log("Block aligned and next block spawned.");
                }
            }
        }
    }

    private float CalculateOverlap(GameObject previousBlock)
    {
        BoxCollider2D previousCollider = previousBlock.GetComponent<BoxCollider2D>();
        Bounds currentBounds = boxCollider.bounds;
        Bounds previousBounds = previousCollider.bounds;

        float overlap = Mathf.Min(currentBounds.max.x, previousBounds.max.x) - Mathf.Max(currentBounds.min.x, previousBounds.min.x);
        overlap = Mathf.Max(overlap, 0);

        return overlap;
    }

    private void AlignWithPreviousBlock(GameObject previousBlock)
    {
        BoxCollider2D previousCollider = previousBlock.GetComponent<BoxCollider2D>();
        Bounds previousBounds = previousCollider.bounds;
        Bounds currentBounds = boxCollider.bounds;

        Vector3 newPosition = transform.position;
        newPosition.x = currentBounds.center.x;
        // newPosition.y = previousBounds.max.y + currentBounds.extents.y;
        transform.position = newPosition;
    }
}
