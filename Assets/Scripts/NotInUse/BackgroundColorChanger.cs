using UnityEngine;

public class BackgroundColorChanger : MonoBehaviour
{
    public Camera mainCamera;
    public Color[] colors;
    public float transitionSpeed = 1.0f; // Speed of color transition
    private int currentColorIndex = 0;
    private int nextColorIndex;
    private float t = 0;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Define pastel colors in the desired flow
        colors = new Color[]
        {
            new Color(1.0f, 0.8f, 0.8f), // Pastel Red
            new Color(1.0f, 0.85f, 0.7f), // Pastel Red-Orange
            new Color(1.0f, 0.87f, 0.68f), // Pastel Orange
            new Color(1.0f, 0.9f, 0.6f), // Pastel Yellow-Orange
            new Color(1.0f, 0.98f, 0.8f), // Pastel Yellow
            new Color(0.9f, 1.0f, 0.8f), // Pastel Yellow-Green
            new Color(0.8f, 1.0f, 0.8f), // Pastel Green
            new Color(0.7f, 1.0f, 0.9f), // Pastel Blue-Green
            new Color(0.6f, 0.8f, 1.0f), // Pastel Blue
            new Color(0.8f, 0.8f, 1.0f), // Pastel Blue-Violet
            new Color(0.8f, 0.68f, 1.0f), // Pastel Violet
            new Color(1.0f, 0.75f, 0.8f) // Pastel Red-Violet
        };

        nextColorIndex = (currentColorIndex + 1) % colors.Length;
        mainCamera.backgroundColor = colors[currentColorIndex];
    }

    void Update()
    {
        t += Time.deltaTime * transitionSpeed;

        if (t >= 1f)
        {
            t = 0f;
            currentColorIndex = nextColorIndex;
            nextColorIndex = (nextColorIndex + 1) % colors.Length;
        }

        mainCamera.backgroundColor = Color.Lerp(colors[currentColorIndex], colors[nextColorIndex], t);
    }
}
