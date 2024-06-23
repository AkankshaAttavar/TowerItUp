using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.IO;

public class EndGamePanelController : MonoBehaviour
{
    public TextMeshProUGUI finalScoreText;
    public Button shareButton;
    public Button mainMenuButton;
    public Button replayButton;
    public TextMeshProUGUI screenshotSavedText; // TextMeshProUGUI for the screenshot saved message

    private string screenshotPath;

    void Start()
    {
        // Ensure these buttons are assigned in the Inspector
        if (shareButton != null) shareButton.onClick.AddListener(TakeScreenshotAndShare);
        if (replayButton != null) replayButton.onClick.AddListener(ReplayGame);
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(GoToMainMenu);

        gameObject.SetActive(false); // Ensure the panel is hidden initially
        if (screenshotSavedText != null) screenshotSavedText.gameObject.SetActive(false); // Hide the screenshot saved text initially
    }

    public void ShowEndGamePanel(int finalScore)
    {
        if (finalScoreText != null)
        {
            finalScoreText.text = "Final Score Is " + finalScore.ToString();
        }
        gameObject.SetActive(true);
    }

    private void TakeScreenshotAndShare()
    {
        StartCoroutine(CaptureScreenshotAndShare());
    }

    private IEnumerator CaptureScreenshotAndShare()
    {
        yield return new WaitForEndOfFrame();

        Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenshot.Apply();

        byte[] bytes = screenshot.EncodeToPNG();
        string downloadsPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), "Downloads");
        screenshotPath = Path.Combine(downloadsPath, "screenshot.png");
        File.WriteAllBytes(screenshotPath, bytes);
        Debug.Log($"Screenshot saved at: {screenshotPath}");

        if (screenshotSavedText != null)
        {
            screenshotSavedText.text = "Screenshot saved in Downloads!";
            screenshotSavedText.gameObject.SetActive(true);
        }

        string finalScore = finalScoreText != null ? finalScoreText.text.Replace("Final Score: ", "") : "N/A";
        string tweet = $"Check out my score in Tower It Up! :- ' Final Score: {finalScore} '.\n\n[Screenshot saved at: {screenshotPath} Delete this message and attach the screenshot manually]";
        string url = "http://twitter.com/intent/tweet?text=" + UnityWebRequest.EscapeURL(tweet);

        Application.OpenURL(url);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ReplayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
