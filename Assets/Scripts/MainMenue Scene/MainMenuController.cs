using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public AudioClip gameSceneClip;
    public AudioClip mainMenuClip;

    private void Start()
    {
        // Play the main menu clip when the scene starts
        AudioManager.instance.PlayAudioClip(mainMenuClip);
    }

    public void PlayGame()
    {
        // Transition to the game scene and switch audio
        AudioManager.instance.PlayAudioClip(gameSceneClip);
        SceneManager.LoadScene("Game");
    }

    public void GoToMainMenu()
    {
        // Transition back to the main menu scene and switch audio
        AudioManager.instance.PlayAudioClip(mainMenuClip);
        SceneManager.LoadScene("MainMenu");
    }
}
