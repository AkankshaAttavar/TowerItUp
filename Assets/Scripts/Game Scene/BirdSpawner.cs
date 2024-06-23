using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdSpawner : MonoBehaviour
{
    public GameObject bird1Prefab; // Bird1 prefab
    public GameObject bird2Prefab; // Bird2 prefab
    public GameObject bird3Prefab; // Bird3 prefab
    public float spawnInterval = 5f; // Time interval between spawns
    public float speed = 2f; // Speed at which birds move
    public float minY = 3f; // Minimum Y position above the ground
    public int maxBirdsInView = 10; // Maximum number of birds allowed in the camera view

    private Camera mainCamera;
    private List<GameObject> activeBirds = new List<GameObject>();

    void Start()
    {
        mainCamera = Camera.main;
        StartCoroutine(SpawnBirds());
    }

    IEnumerator SpawnBirds()
    {
        while (true)
        {
            if (activeBirds.Count < maxBirdsInView)
            {
                SpawnBird();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnBird()
    {
        // Randomly choose a bird prefab
        GameObject birdPrefab = ChooseBirdPrefab();
        GameObject bird = Instantiate(birdPrefab);

        // Determine spawn position and direction
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;
        float randomY = Random.Range(mainCamera.transform.position.y + minY, mainCamera.transform.position.y + cameraHeight / 2);

        float spawnX = Random.Range(0, 2) == 0 ? mainCamera.transform.position.x - cameraWidth / 2 - 1 : mainCamera.transform.position.x + cameraWidth / 2 + 1;
        Vector2 velocity = spawnX < mainCamera.transform.position.x ? Vector2.right * speed : Vector2.left * speed;

        bird.transform.position = new Vector3(spawnX, randomY, 0);
        bird.GetComponent<Rigidbody2D>().velocity = velocity;

        // Ensure bird2 and bird3 always move from right to left, and bird1 moves from left to right
        if (birdPrefab == bird1Prefab && velocity.x < 0)
        {
            FlipBird(bird);
            bird.GetComponent<Rigidbody2D>().velocity = Vector2.right * speed;
        }
        else if ((birdPrefab == bird2Prefab || birdPrefab == bird3Prefab) && velocity.x > 0)
        {
            FlipBird(bird);
            bird.GetComponent<Rigidbody2D>().velocity = Vector2.left * speed;
        }

        activeBirds.Add(bird);
        StartCoroutine(RemoveBirdWhenOutOfView(bird));
    }

    GameObject ChooseBirdPrefab()
    {
        int randomIndex = Random.Range(0, 3);
        switch (randomIndex)
        {
            case 0: return bird1Prefab;
            case 1: return bird2Prefab;
            case 2: return bird3Prefab;
            default: return bird1Prefab;
        }
    }

    void FlipBird(GameObject bird)
    {
        Vector3 birdScale = bird.transform.localScale;
        birdScale.x *= -1;
        bird.transform.localScale = birdScale;
    }

    IEnumerator RemoveBirdWhenOutOfView(GameObject bird)
    {
        while (bird != null)
        {
            float cameraHeight = 2f * mainCamera.orthographicSize;
            float cameraWidth = cameraHeight * mainCamera.aspect;

            if (bird.transform.position.x < mainCamera.transform.position.x - cameraWidth / 2 - 1 ||
                bird.transform.position.x > mainCamera.transform.position.x + cameraWidth / 2 + 1)
            {
                activeBirds.Remove(bird);
                Destroy(bird);
                yield break;
            }
            yield return null;
        }
    }
}
