using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform hook; // Reference to the hook
    public float offsetY = 5f; // Offset for the camera position relative to the hook
    public float smoothSpeed = 0.125f; // Speed of the smooth camera movement

    void LateUpdate()
    {
        if (hook != null)
        {
            // Calculate the desired position below the hook
            Vector3 desiredPosition = new Vector3(
                transform.position.x,
                hook.position.y - offsetY,
                transform.position.z
            );

            // Smoothly move the camera to the desired position
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }



    public IEnumerator PanToPosition(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position;
        Vector3 endPosition = new Vector3(transform.position.x, targetPosition.y, transform.position.z);
        float elapsedTime = 0f;
        float duration = 2f; // Duration of the pan

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;
    }

}
