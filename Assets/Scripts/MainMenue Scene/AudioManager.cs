using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    private AudioSource audioSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayAudioClip(AudioClip newClip, float transitionTime = 1.0f)
    {
        StartCoroutine(FadeOutIn(newClip, transitionTime));
    }

    private IEnumerator FadeOutIn(AudioClip newClip, float transitionTime)
    {
        if (audioSource.isPlaying)
        {
            for (float t = 0; t < transitionTime; t += Time.deltaTime)
            {
                audioSource.volume = 1 - t / transitionTime;
                yield return null;
            }
            audioSource.Stop();
        }

        audioSource.clip = newClip;
        audioSource.Play();

        for (float t = 0; t < transitionTime; t += Time.deltaTime)
        {
            audioSource.volume = t / transitionTime;
            yield return null;
        }

        audioSource.volume = 1;
    }
}
