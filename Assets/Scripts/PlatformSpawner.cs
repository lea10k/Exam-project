// PlatformSpawner.cs
// Spawns random platforms for an endless vertical 2D platformer game

using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] platformPrefabs; // Array of different platform prefabs (0 = static, 1+ = dynamic)

    [Header("Playfield")]
    public Transform player; // Reference to the player (used to spawn platforms ahead of player)

    [Header("Spacing Rules")]
    public float verticalSpacing = 2.5f; // Minimum vertical gap between platforms
    public float horizontalDistance = 3f; // Horizontal spacing between platforms

    // Internal tracking variables
    private float lastY = 0f; // Y position of the last spawned platform
    private float lastX = 0f; // X position of the last spawned platform
    private int dynamicCount = 0; // Counter for dynamic platforms in current group
    private int lastIndex = 0; // Index of last spawned platform prefab

    void Start()
    {   
        // Spawn initial static platform at (5,0) to start the game
        Instantiate(platformPrefabs[0], new Vector2(5f, 0f), Quaternion.identity);
        lastY = 0f; // Set initial Y position
        lastX = 5f; // Set initial X position
        lastIndex = 0; // First platform is static (index 0)
        
        // Generate first 10 platforms to populate the starting area
        for (int i = 0; i < 10; i++) GeneratePlatform();
    }

    void Update()
    {
        // Check if player has moved close to the last platform (10 units below)
        if (player.position.y + 10f > lastY)
        {
            GeneratePlatform(); // Spawn new platform when needed
        }
    }

    // Counters for platform generation logic
    int counterNeg = 0; // Counts consecutive left movements
    int counterPos = 0; // Counts consecutive right movements
    int spawnCount = 0; // Total platforms spawned
    int fallingCount = 0; // Counts falling platforms in current group

    void GeneratePlatform()
    {
        int index; // Will store selected platform type index

        // Every 5th platform: allow dynamic moving platforms
        if (spawnCount > 0 && spawnCount % 5 == 0)
        {
            // Choose randomly between horizontal movers (index 3,4) or vertical mover (5)
            int[] dynamicOptions = { 3, 4, 5 };
            index = dynamicOptions[Random.Range(0, dynamicOptions.Length)];
        }
        else
        {
            // For normal platforms, choose between static (0), disappearing (1), and falling (2)
            // Only allow falling platforms if we haven't exceeded limit (fallingCount < 1)
            int[] normalOptions = (fallingCount < 1) ? new int[] { 0, 1, 2 } : new int[] { 0, 1 };
            index = normalOptions[Random.Range(0, normalOptions.Length)];
        }

        // Increment falling platform counter if we spawned one
        if (index == 2)
        {
            fallingCount++;
        }

        // Reset counters every 5 platforms
        if (spawnCount % 5 == 0)
        {
            fallingCount = 0;
        }

        // Get the selected platform prefab
        GameObject prefab = platformPrefabs[index];

        // Calculate spawn position
        float spawnX = lastX;
        float maxPath = 10f; // Maximum horizontal movement range

        // Check if we can move left or right based on counters
        bool canGoLeft = counterNeg < 3;
        bool canGoRight = counterPos < 3;

        // Handle platform positioning logic
        if (lastX >= maxPath) // If at right edge
        {
            spawnX = lastX - horizontalDistance; // Move left
            counterNeg++;
            counterPos = 0;
        }
        else if (lastX <= -maxPath) // If at left edge
        {
            spawnX = lastX + horizontalDistance; // Move right
            counterPos++;
            counterNeg = 0;
        }
        else if (lastX == 0) // If centered
        {
            if (canGoLeft && canGoRight) // Can go either direction
            {
                if (Random.value < 0.5f) // Random choice
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
            else if (canGoLeft) // Only left allowed
            {
                spawnX = lastX - horizontalDistance;
                counterNeg++;
                counterPos = 0;
            }
            else // Only right allowed
            {
                spawnX = lastX + horizontalDistance;
                counterPos++;
                counterNeg = 0;
            }
        }
        else // Not at edge or center
        {
            // Same logic as centered case for movement options
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

        // Calculate Y position (always above last platform)
        float spawnY = lastY + verticalSpacing;

        // Special adjustments for moving platforms
        if (index == 3) // Moving Horizontal B
        {
            spawnX += 4f; // Offset to the right
        }
        else if (index == 4) // Moving Horizontal A
        {
            spawnX += -4f; // Offset to the left
        }
        else if (index == 5) // Moving Vertical
        {
            spawnY += 1f; // Start slightly higher
        }

        // Create the platform at calculated position
        Vector2 spawnPos = new Vector2(spawnX, spawnY);
        Instantiate(prefab, spawnPos, Quaternion.identity);

        // Update tracking variables
        lastX = spawnX;
        lastY = spawnY;
        lastIndex = index;
        spawnCount++;
    }

    // Alternative platform selection method (currently unused)
    int SelectPlatformIndex()
    {
        bool allowDynamic = dynamicCount < 2; // Only allow dynamic if under limit

        // Keep trying until we find a valid platform
        while (true)
        {
            int index = Random.Range(0, platformPrefabs.Length);
            if (index == 0) return index; // Always allow static
            if (allowDynamic && index != lastIndex) return index; // Allow dynamic if not repeating
        }
    }
}