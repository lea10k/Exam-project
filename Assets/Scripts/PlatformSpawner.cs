// PlatformSpawner.cs
// Spawns random platforms for an endless vertical 2D platformer game

using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] platformPrefabs; // Array of different platform prefabs (0 = static, 1+ = dynamic)

    [Header("Playfield")]
    public float horizontalRange = 10f; // How far left and right platforms can spawn
    public float fieldWidth = 24f; // Total playfield width
    public Transform player; // Reference to the player (used to spawn ahead)

    [Header("Jump Physics")]
    public float maxJumpHeight = 8f; // Max vertical reach of a player jump
    public float maxJumpWidth = 5f;  // Max horizontal jump distance

    [Header("Spacing Rules")]
    public float minVerticalSpacing = 2.5f; // Min vertical gap between platforms
    public float maxVerticalSpacing = 5.5f; // Max vertical gap between platforms
    public float minHorizontalDistance = 2.5f; // Platforms can't spawn too close horizontally

    // Internal tracking variables
    private float lastY = 0f; // Last platform's Y position
    private float lastX = 0f; // Last platform's X position
    private int spawnCounter = 0; // Tracks how many platforms since last dynamic reset
    private int dynamicCount = 0; // How many dynamic platforms spawned in last group of 5
    private int lastIndex = -1; // Last used platform prefab index

    void Start()
    {
        // Spawn 10 platforms at the beginning
        for (int i = 0; i < 10; i++) GeneratePlatform();
    }

    void Update()
    {
        // If player gets close to the last Y height, spawn more platforms
        if (player.position.y + 10f > lastY)
        {
            GeneratePlatform();
        }
    }

    void GeneratePlatform()
    {
        int index = SelectPlatformIndex(); // Choose which platform prefab to use
        GameObject prefab = platformPrefabs[index];

        // Pick a new X position, make sure it's not too close to the last X
        float spawnX;
        int maxTries = 10;
        int tries = 0;
        do {
            spawnX = Random.Range(-horizontalRange, horizontalRange);
            tries++;
        } while (Mathf.Abs(spawnX - lastX) < minHorizontalDistance && tries < maxTries);
        lastX = spawnX;

        // Choose random Y distance between min and max spacing
        float deltaY = Random.Range(minVerticalSpacing, maxVerticalSpacing);
        float spawnY = lastY + deltaY;

        float movementHeight = 0f;

        // If it's a vertical moving platform, get its movement height and adjust spawnY
        if (index == 2)
        {
            movementHeight = GetMaxYFromPrefab(prefab);
            spawnY = Mathf.Max(spawnY, lastY + movementHeight);
        }

        // If it's a horizontal moving platform, check its range and adjust spawnY/X if needed
        if (index == 1)
        {
            float movementWidth = GetMaxXFromPrefab(prefab);
            float halfField = fieldWidth / 2f;

            if (Mathf.Abs(spawnX) + movementWidth > halfField)
            {
                spawnX = Mathf.Sign(spawnX) * (halfField - movementWidth);
            }

            movementHeight = GetMaxYFromPrefab(prefab);
            spawnY = Mathf.Max(spawnY, lastY + movementHeight);
        }

        Vector2 spawnPos = new Vector2(spawnX, spawnY);
        Instantiate(prefab, spawnPos, Quaternion.identity); // Spawn the chosen platform

        // If the vertical gap is too big, add a helper static platform in between
        float actualGap = spawnY - lastY;
        if (actualGap > maxJumpHeight)
        {
            float helperY = lastY + actualGap / 2f;
            float helperX = Random.Range(-maxJumpWidth, maxJumpWidth);
            Vector2 helperPos = new Vector2(helperX, helperY);
            Instantiate(platformPrefabs[0], helperPos, Quaternion.identity);
        }

        // Update trackers
        lastY = spawnY;
        lastIndex = index;
        spawnCounter++;
        if (IsDynamic(index)) dynamicCount++;

        // Reset every 5 spawns
        if (spawnCounter >= 5)
        {
            spawnCounter = 0;
            dynamicCount = 0;
        }
    }

    // Randomly choose a platform index, respecting limits for dynamic platforms
    int SelectPlatformIndex()
    {
        bool allowDynamic = dynamicCount < 2;

        while (true)
        {
            int index = Random.Range(0, platformPrefabs.Length);
            if (index == 0) return index; // Static platform is always allowed
            if (allowDynamic && index != lastIndex) return index; // Avoid repeats
        }
    }

    // Calculate vertical movement span of a platform prefab (Pos A ↔ Pos B)
    float GetMaxYFromPrefab(GameObject prefab, string aName = "Pos A", string bName = "Pos B")
    {
        Transform a = prefab.transform.Find(aName);
        Transform b = prefab.transform.Find(bName);
        if (a != null && b != null)
            return Mathf.Abs(b.localPosition.y - a.localPosition.y);
        return 0f;
    }

    // Calculate horizontal movement span of a platform prefab
    float GetMaxXFromPrefab(GameObject prefab, string aName = "Pos A", string bName = "Pos B")
    {
        Transform a = prefab.transform.Find(aName);
        Transform b = prefab.transform.Find(bName);
        if (a != null && b != null)
            return Mathf.Abs(b.localPosition.x - a.localPosition.x) / 2f;
        return 0f;
    }

    // Returns true if the platform is dynamic (anything except index 0)
    bool IsDynamic(int index)
    {
        return index != 0;
    }
}