using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    public GameObject objectToSpawn; // This is the slot you're looking for!
    public int numberOfObjects = 50;
    public float radius = 20.0f;

    // --- Fix for Volume/Height Feel ---
    public float minHeight = 1.0f;
    public float maxHeight = 5.0f;

    void Start()
    {
        if (objectToSpawn == null)
        {
            Debug.LogError("RandomSpawner: ObjectToSpawn is unassigned!");
            return;
        }

        for (int i = 0; i < numberOfObjects; i++)
        {
            SpawnRandomly();
        }
    }

    void SpawnRandomly()
    {
        // Calculate random position within the circle (X and Z)
        Vector2 randomCircle = Random.insideUnitCircle * radius;

        // Set height (Y) to be random between min and max height
        float randomY = Random.Range(minHeight, maxHeight);

        // Combine into final position
        Vector3 spawnPos = new Vector3(randomCircle.x, randomY, randomCircle.y);

        Quaternion randomRot = Quaternion.Euler(0, Random.Range(0, 360), 0);

        // Instantiate the object and ensure it is not a child of the spawner
        Instantiate(objectToSpawn, spawnPos, randomRot);
    }
}