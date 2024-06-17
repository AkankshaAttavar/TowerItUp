using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    public float swingSpeed = 2f;    // Speed of the hook swing
    public float swingRadius = 2f;   // Radius of the swing
    private float time;
    private Transform currentBlock;

    void Update()
    {
        SwingHook();
    }

    void SwingHook()
    {
        time += Time.deltaTime * swingSpeed;
        float xPosition = Mathf.Sin(time) * swingRadius;
        transform.position = new Vector3(xPosition, transform.position.y, transform.position.z);

        if (currentBlock != null && !currentBlock.GetComponent<TowerBlock>().isReleased)
        {
            currentBlock.position = new Vector3(transform.position.x, currentBlock.position.y, currentBlock.position.z);
        }
    }

    public void AttachBlock(Transform block)
    {
        currentBlock = block;
    }

    public void ReleaseBlock()
    {
        if (currentBlock != null)
        {
            currentBlock = null;
        }
    }
}
