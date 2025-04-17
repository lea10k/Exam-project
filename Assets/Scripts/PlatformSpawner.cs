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
    public float verticalSpacing = 2.5f; // Min vertical gap between platforms
    public float horizontalDistance = 3f; // Platforms can't spawn too close horizontally

    // Internal tracking variables
    private float lastY = 0f; // Last platform's Y position
    private float lastX = 0f; // Last platform's X position
    private int spawnCounter = 0; // Tracks how many platforms since last dynamic reset
    private int dynamicCount = 0; // How many dynamic platforms spawned in last group of 5
    private int lastIndex = -1; // Last used platform prefab index

    void Start()
{   
    Instantiate(platformPrefabs[0], new Vector2(5f, 0f), Quaternion.identity); // Spawn the first static platform at (0,0)
    lastY = 0f; // Reset last Y position
    lastX = 5f;
    lastIndex = 0; // Reset last index to static platform
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

int counterNeg = 0; // Number of consecutive left spawns
int counterPos = 0; // Number of consecutive right spawns

void GeneratePlatform()
{
    int index = SelectPlatformIndex();
    GameObject prefab = platformPrefabs[index];

    float spawnX = lastX;
    float maxPath = 10f;

    bool canGoLeft = counterNeg < 3;
    bool canGoRight = counterPos < 3;

    if (lastX >= maxPath)
    {
        // Too far right, force spawn left
        spawnX = lastX - horizontalDistance;
        counterNeg++;
        counterPos = 0;
    }
    else if (lastX <= -maxPath)
    {
        // Too far left, force spawn right
        spawnX = lastX + horizontalDistance;
        counterPos++;
        counterNeg = 0;
    }
    else if (lastX == 0)
    {
        // Centered → pick random direction, but respect counters
        if (canGoLeft && canGoRight)
        {
            if (Random.value < 0.5f)
            {
                spawnX = lastX - horizontalDistance;
                counterNeg++;
                counterPos = 0;
            }
            else
            {
                spawnX = lastX + horizontalDistance;
                counterPos++;
                counterNeg = 0;
            }
        }
        else if (canGoLeft)
        {
            spawnX = lastX - horizontalDistance;
            counterNeg++;
            counterPos = 0;
        }
        else
        {
            spawnX = lastX + horizontalDistance;
            counterPos++;
            counterNeg = 0;
        }
    }
    else
    {
        // In bounds → prefer random, but block direction if counter reached
        if (canGoLeft && canGoRight)
        {
            if (Random.value < 0.5f)
            {
                spawnX = lastX - horizontalDistance;
                counterNeg++;
                counterPos = 0;
            }
            else
            {
                spawnX = lastX + horizontalDistance;
                counterPos++;
                counterNeg = 0;
            }
        }
        else if (canGoLeft)
        {
            spawnX = lastX - horizontalDistance;
            counterNeg++;
            counterPos = 0;
        }
        else
        {
            spawnX = lastX + horizontalDistance;
            counterPos++;
            counterNeg = 0;
        }
    }

    float spawnY = lastY + verticalSpacing;
    Vector2 spawnPos = new Vector2(spawnX, spawnY);
    Instantiate(prefab, spawnPos, Quaternion.identity);

    lastX = spawnX;
    lastY = spawnY;
    lastIndex = index;
}

    // Randomly choose a platform index, respecting limits for dynamic platforms
    int SelectPlatformIndex()
    {
        bool allowDynamic = dynamicCount < 2; // max 2 dynamic platforms in 5 spawns

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
            return Mathf.Abs(b.localPosition.x - a.localPosition.x) / 2f; // Dividing by 2 for half-width of field, because platforms are centered (0,0)
        // If no movement reference, return 0
        return 0f;
    }

    // Returns true if the platform is dynamic (anything except index 0)
    bool IsDynamic(int index)
    {
        return index != 0;
    }
}