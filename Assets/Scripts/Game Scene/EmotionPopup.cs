using System.Collections;
using UnityEngine;

public class EmotionPopup : MonoBehaviour
{
    public float displayTime = 2f;

    void Start()
    {
        StartCoroutine(ShowAndDestroy());
    }

    private IEnumerator ShowAndDestroy()
    {
        yield return new WaitForSeconds(displayTime);
        Destroy(gameObject);
    }
}
