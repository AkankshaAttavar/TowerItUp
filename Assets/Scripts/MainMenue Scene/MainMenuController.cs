using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    private void Start()
    {
        // Any initialization code if needed
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void GoToEndless()
    {
        SceneManager.LoadScene("Endless");
    }

    public void GoToConnectWallet()
    {
        SceneManager.LoadScene("ConnectWallet");
    }
}
