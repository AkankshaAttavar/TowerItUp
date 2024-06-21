using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    public float swingSpeed = 2f; // Speed of the hook swing
    public float swingRadiusX = 2f; // Horizontal radius of the swing
    private float time;
    private Transform currentBlock;
    private Vector3 targetPosition;

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        SwingHook();
        SmoothMove();
    }

    void SwingHook()
    {
        time += Time.deltaTime * swingSpeed;
        float xPosition = Mathf.Sin(time) * swingRadiusX; // Horizontal pendulum motion
        targetPosition = new Vector3(xPosition, targetPosition.y, transform.position.z);

        if (currentBlock != null && !currentBlock.GetComponent<TowerBlock>().isReleased)
        {
            currentBlock.position = new Vector3(transform.position.x, transform.position.y, currentBlock.position.z);
        }
    }

    void SmoothMove()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * swingSpeed);
    }

    public void AttachBlock(Transform block)
    {
        currentBlock = block;
    }

    public void ReleaseBlock()
    {
        if (currentBlock != null)
        {
            currentBlock.GetComponent<TowerBlock>().isReleased = true;
            currentBlock = null;
        }
    }

    public void AdjustHookPosition(Vector3 newPosition)
    {
        targetPosition = newPosition;
    }

    public void IncreaseRadiusAndSpeed(float radiusIncrement, float speedIncrement)
    {
        swingRadiusX += radiusIncrement;
        swingSpeed += speedIncrement;
    }
}
