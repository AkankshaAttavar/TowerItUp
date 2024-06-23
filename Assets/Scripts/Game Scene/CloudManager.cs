using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    public GameObject[] cloudPrefabs; // Array to hold your cloud prefabs
    public float spawnInterval = 5f; // Time interval between spawns
    public float speed = 2f; // Speed at which clouds move
    public float minY = 5f; // Minimum Y position above the ground
    public int maxCloudsInView = 6; // Maximum number of clouds allowed in the camera view

    private Camera mainCamera;
    private List<GameObject> activeClouds = new List<GameObject>();

    void Start()
    {
        mainCamera = Camera.main;
        StartCoroutine(SpawnClouds());
    }

    IEnumerator SpawnClouds()
    {
        while (true)
        {
            if (activeClouds.Count < maxCloudsInView)
            {
                SpawnCloud();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnCloud()
    {
        // Choose a random cloud prefab
        int randomIndex = Random.Range(0, cloudPrefabs.Length);
        GameObject cloud = Instantiate(cloudPrefabs[randomIndex]);

        // Determine spawn position
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        float randomY = Random.Range(mainCamera.transform.position.y + minY, mainCamera.transform.position.y + cameraHeight / 2);

        // Randomly choose left or right side for spawn
        float randomX = Random.Range(0, 2) == 0 ?
            mainCamera.transform.position.x - cameraWidth / 2 - 1 :
            mainCamera.transform.position.x + cameraWidth / 2 + 1;

        cloud.transform.position = new Vector3(randomX, randomY, 0);

        // Move cloud to the other side
        float targetX = randomX > mainCamera.transform.position.x ?
            mainCamera.transform.position.x - cameraWidth / 2 - 1 :
            mainCamera.transform.position.x + cameraWidth / 2 + 1;

        cloud.GetComponent<Rigidbody2D>().velocity = new Vector2(targetX > randomX ? speed : -speed, 0);

        // Add the cloud to the active list and set up a callback to remove it when it exits the camera view
        activeClouds.Add(cloud);
        StartCoroutine(RemoveCloudWhenOutOfView(cloud));
    }

    IEnumerator RemoveCloudWhenOutOfView(GameObject cloud)
    {
        while (cloud != null)
        {
            float cameraHeight = 2f * mainCamera.orthographicSize;
            float cameraWidth = cameraHeight * mainCamera.aspect;

            if (cloud.transform.position.x < mainCamera.transform.position.x - cameraWidth / 2 - 1 ||
                cloud.transform.position.x > mainCamera.transform.position.x + cameraWidth / 2 + 1)
            {
                activeClouds.Remove(cloud);
                Destroy(cloud);
                yield break;
            }
            yield return null;
        }
    }
}
