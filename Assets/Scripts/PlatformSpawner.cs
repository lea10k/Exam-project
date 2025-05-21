using System.Collections.Generic;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [Tooltip("Array of platform prefabs (0 = static, 1 = disappearing, 2 = falling, 3-4 = moving)")]
    public GameObject[] platformPrefabs;

    [Header("Playfield")]
    public Transform player;
    [Tooltip("Total playfield width")]
    public float playfieldWidth = 26f;
    [Tooltip("Maximum horizontal movement range (between -maxPath and +maxPath)")]
    public float maxPath = 11f; 
    
    [Tooltip("Manual Platform width only used if autoDetect = false")]
    public float manualPlatformWidth = 4.4f;

    [Header("Spacing Rules")]
    public float verticalSpacing = 2.5f;
    [Tooltip("Horizontal distance to last spawned and new spawned platform")]
    public float horizontalDistance = 2f;

    [Header("Generation Settings")]
    [Tooltip("How far ahead of player to generate platforms")]
    public float generationLookAhead = 10f;
    [Tooltip("Maximum consecutive platforms in same direction")]
    public int maxConsecutiveDirections = 4;

    [Header("Monster Settings")]
    [Tooltip("Monster prefabs (0 = Shark, 1 = GreenSlime)")]
    public GameObject[] monsterPrefabs;
    [Tooltip("Chance to spawn a monster on a static platform (0 to 1)")]
    [Range(0f, 1f)]
    public float monsterSpawnChance = 0.3f;

    [Header("Power-Up Settings")]
    [Tooltip("Power-up prefabs")]
    public GameObject[] powerUpPrefabs;
    [Tooltip("Chance to spawn a power-up on a static platform (0 to 1)")]
    [Range(0f, 1f)]
    public float powerUpSpawnChance = 0.2f;

    // Platform type constants for better readability
    private enum PlatformType
    {
        Static = 0,
        Disappearing = 1,
        Falling = 2,
        MovingHorizontal = 3,
        MovingVertical = 4
    }

    // Internal tracking variables
    private float lastSpawnedY = 0f;
    private float lastSpawnedX = 0f; // Start at the center (x=0)
    private bool wasLastPlatformMoving = false;
    private int consecutiveLeftMoves = 0;
    private int consecutiveRightMoves = 0;
    private int totalPlatformsSpawned = 0;
    private int fallingPlatformsInGroup = 0;
    private int disappearingPlatformsInGroup = 0;
    private bool wasLastPlatformVertical = false; 
    private bool firstSpawn = false;
    private float platformWidth;

    void Start()
    {
        InitializePlatformWidth();
        ValidatePlayfieldSettings();
        
        // Spawn initial platform and setup starting state
        SpawnInitialPlatform();
        firstSpawn = true;
        
        // Generate initial platforms
        for (int i = 0; i < 30; i++)
        {
            GenerateNextPlatform();
        }
    }

    private void InitializePlatformWidth()
    {
        // Try to detect platform width from the first platform prefab
        Renderer renderer = platformPrefabs[0].GetComponent<Renderer>();
        if (renderer != null)
        {
            // Get width from the renderer's bounds
            platformWidth = renderer.bounds.size.x;
            Debug.Log($"Auto-detected platform width: {platformWidth}");
        } 
        else
        {
            // Use the manual platform width
            platformWidth = manualPlatformWidth;
        }
    }

    private void ValidatePlayfieldSettings()
    {
        // Make sure maxPath doesn't exceed half the playfield width
        float halfPlayfieldWidth = playfieldWidth / 2;
        
        if (maxPath > halfPlayfieldWidth)
        {
            Debug.LogWarning($"maxPath ({maxPath}) exceeds half of playfield width ({halfPlayfieldWidth}). Adjusting to fit.");
            float distanceOfFieldEndToMaxPath = 2;
            maxPath = halfPlayfieldWidth - distanceOfFieldEndToMaxPath;
        } 
        else
        {
            Debug.LogWarning($"maxPath is ({maxPath})");
        }
    }

    void Update()
    {
        // Generate new platforms when player approaches the highest one
        if (ShouldGenerateMorePlatforms())
        {
            GenerateNextPlatform();
        }
    }

    private void SpawnInitialPlatform()
    {
        // Create the first static platform at the center (x=0)
        Vector2 startPosition = new Vector2(0f, 0f);
        Instantiate(platformPrefabs[(int)PlatformType.Static], startPosition, Quaternion.identity);

        // Initialize tracking variables
        lastSpawnedX = startPosition.x;
        lastSpawnedY = startPosition.y;

        UpdateTrackingVariables((int)PlatformType.Static, startPosition);
        Debug.Log(totalPlatformsSpawned + "platforms spawed at first");

    }

    private bool ShouldGenerateMorePlatforms()
    {
        // Check if player is getting close to the highest platform
        return player.position.y + generationLookAhead > lastSpawnedY;
    }

    private void GenerateNextPlatform()
    {
        // Step 1: Select platform type
        int platformIndex = SelectPlatformType();

        // Step 2: Calculate spawn position
        Vector2 spawnPosition = CalculateSpawnPosition(platformIndex);

        // Step 3: Create the platform
        GameObject platform = Instantiate(platformPrefabs[platformIndex], spawnPosition, Quaternion.identity);

        // Step 4: Spawn monster or power-up if platform is static
        if (platformIndex == (int)PlatformType.Static)
        {
            TrySpawnMonsterOrPowerUp(spawnPosition, platform);
        }

        // Step 5: Update tracking variables
        UpdateTrackingVariables(platformIndex, spawnPosition);
        Debug.Log(totalPlatformsSpawned + "platforms spawned");
    }

    private int ChooseMovingPlatformType()
    {
        // Choose randomly between horizontal movers or vertical mover
        int[] movingOptions = {
                (int)PlatformType.MovingHorizontal,
                (int)PlatformType.MovingVertical
            };

        return movingOptions[Random.Range(0, movingOptions.Length)];
    }

    private int SelectPlatformType()
    {
        // If last platform was moving, force a static platform next
        if (wasLastPlatformMoving)
        {
            wasLastPlatformMoving = false;
            return (int)PlatformType.Static;
        }

        // Every 5th platform, spawn a moving platform
        if (firstSpawn == true && totalPlatformsSpawned > 0 && totalPlatformsSpawned % 4 == 0)
        {
            firstSpawn = false;
            return ChooseMovingPlatformType();
        }
        else if (firstSpawn == false && totalPlatformsSpawned > 0 && totalPlatformsSpawned % 5 == 4)
        {
            return ChooseMovingPlatformType();
        }
        else
        {
            // Only one falling platform allowed per group of 5 spawned platforms
            bool canSpawnFalling = fallingPlatformsInGroup < 1;
            // Allow at most two disappearing platforms in a row
            bool canSpawnDisappearing = disappearingPlatformsInGroup < 2;

            List<int> options = new() { (int)PlatformType.Static };

            if (canSpawnDisappearing)
                options.Add((int)PlatformType.Disappearing);

            if (canSpawnFalling)
                options.Add((int)PlatformType.Falling);

            return options[Random.Range(0, options.Count)];
        }
    }

    private Vector2 CalculateSpawnPosition(int platformIndex)
    {
        // Calculate horizontal position based on movement constraints
        float spawnX = CalculateHorizontalPosition();
        float verticalSpaceFromPosB = 1f;
        float spawnY;
        
        // Calculate vertical position (always above last platform)
        if (wasLastPlatformVertical)
        {
            spawnY = lastSpawnedY + verticalSpacing + verticalSpaceFromPosB;
        }
        else
        {
            spawnY = lastSpawnedY + verticalSpacing;
        }
        // Apply platform-specific position adjustments
        Vector2 spawnPosition = AdjustPositionForPlatformType(platformIndex, spawnX, spawnY);
        
        return spawnPosition;
    }
    
    private float CalculateHorizontalPosition()
    {
        float nextX = lastSpawnedX;

        // Determine a limit of right or left spawning platforms
        bool canMoveLeft = consecutiveLeftMoves < maxConsecutiveDirections;
        bool canMoveRight = consecutiveRightMoves < maxConsecutiveDirections;

        // Computes edge boundaries from maxPath to platformwidth / 2
        float edgeBuffer = platformWidth / 2;
        float leftBoundary = -maxPath + edgeBuffer;
        float rightBoundary = maxPath - edgeBuffer;
        
        // Treat edge cases 
        if (lastSpawnedX >= rightBoundary)
        {
            // At right edge -> move to the left
            nextX = lastSpawnedX - horizontalDistance;
            consecutiveLeftMoves++;
            consecutiveRightMoves = 0;
        }
        else if (lastSpawnedX <= leftBoundary)
        {
            // At left edge -> move to the right
            nextX = lastSpawnedX + horizontalDistance;
            consecutiveRightMoves++;
            consecutiveLeftMoves = 0;
        }
        else
        {
            // Normal movement logic
            if (canMoveLeft && canMoveRight)
            {
                bool goRight = Random.value < 0.5f;

                nextX = goRight ? 
                    lastSpawnedX + horizontalDistance : 
                    lastSpawnedX - horizontalDistance;
                
                if (goRight) 
                { 
                    consecutiveRightMoves++; 
                    consecutiveLeftMoves = 0; 
                }
                else 
                { 
                    consecutiveLeftMoves++; 
                    consecutiveRightMoves = 0; 
                }
            }
            else if (canMoveLeft)
            {
                nextX = lastSpawnedX - horizontalDistance;
                consecutiveLeftMoves++;
                consecutiveRightMoves = 0;
            }
            else
            {
                nextX = lastSpawnedX + horizontalDistance;
                consecutiveRightMoves++;
                consecutiveLeftMoves = 0;
            }
        }
        
        // Final limitation
        // if nextX < leftBoundary --> leftBoundary
        // if nextX > rightBoundary --> rightBoundary
        // Otherwise nextX keeps unaltered 
        return Mathf.Clamp(nextX, leftBoundary, rightBoundary);
    }
    
    private Vector2 AdjustPositionForPlatformType(int platformIndex, float x, float y)
    {
        float adjustedX = x;
        float adjustedY = y;
        
        // Computes edge boundaries from maxPath to platformwidth / 2
        float edgeBuffer = platformWidth / 2;
        float leftBoundary = -maxPath + edgeBuffer;
        float rightBoundary = maxPath - edgeBuffer;
        
        // Movement range for horizontally moving platforms 
        const float movementRange = 3f;
        
        switch (platformIndex)
        {
            case (int)PlatformType.MovingHorizontal:
                // Ensure possible movement to the right (+3)
                if ((adjustedX + movementRange) > rightBoundary)
                {
                    adjustedX = rightBoundary - movementRange;
                }
                // Ensure possible movement to the left (-3)
                else if ((adjustedX - movementRange) < leftBoundary)
                {
                    adjustedX = leftBoundary + movementRange;
                }
                break;  
            case (int)PlatformType.MovingVertical:
                adjustedY += 1f; // Start slightly raised 
                break;
        }
        
        // Final positioning 
        return new Vector2(
            Mathf.Clamp(adjustedX, leftBoundary, rightBoundary),
            adjustedY
        );
    }

    private void UpdateTrackingVariables(int platformIndex, Vector2 spawnPosition)
    {
        // Update position tracking
        lastSpawnedX = spawnPosition.x;
        lastSpawnedY = spawnPosition.y;

        // Update platform type tracking
        wasLastPlatformMoving = IsMovingPlatform(platformIndex);
        wasLastPlatformVertical = platformIndex == (int)PlatformType.MovingVertical;

        // Update falling platform counter
        if (platformIndex == (int)PlatformType.Falling)
        {
            fallingPlatformsInGroup++;
        }

        if (platformIndex == (int)PlatformType.Disappearing)
        {
            disappearingPlatformsInGroup++;
        }
        else
        {
            disappearingPlatformsInGroup = 0;
        }
        
        // Increment total counter and reset group counters if needed
        totalPlatformsSpawned++;
        if (totalPlatformsSpawned % 5 == 0)
        {
            fallingPlatformsInGroup = 0;
        }
    }
    
    private bool IsMovingPlatform(int platformIndex)
    {
        // Check if platform is any of the moving types
        return platformIndex >= (int)PlatformType.MovingHorizontal && 
               platformIndex <= (int)PlatformType.MovingVertical;
    }

    private void TrySpawnMonsterOrPowerUp(Vector2 platformPosition, GameObject platform)
    {
        // Ensure monsterPrefabs and powerUpPrefabs arrays are not null or empty
        bool canSpawnMonster = monsterPrefabs != null && monsterPrefabs.Length > 0;
        bool canSpawnPowerUp = powerUpPrefabs != null && powerUpPrefabs.Length > 0;

        // Decide whether to spawn a monster, a power-up, or nothing
        float randomValue = Random.value;

        if (canSpawnMonster && randomValue <= monsterSpawnChance)
        {
            // Spawn a monster
            GameObject monsterPrefab = monsterPrefabs[Random.Range(0, monsterPrefabs.Length)];
            Vector2 monsterPosition = new Vector2(platformPosition.x, platformPosition.y + 0.3f);
            Instantiate(monsterPrefab, monsterPosition, Quaternion.identity, platform.transform);
        }
        else if (canSpawnPowerUp && randomValue <= (monsterSpawnChance + powerUpSpawnChance))
        {
            // Spawn a power-up 5 units higher than the platform
            GameObject powerUpPrefab = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];
            Vector2 powerUpPosition = new Vector2(platformPosition.x, platformPosition.y + 1f);
            Instantiate(powerUpPrefab, powerUpPosition, Quaternion.identity, platform.transform);
        }
    }

    // For debugging
    void OnDrawGizmos()
    {
        // Only show gizmos if initialized
        if (!Application.isPlaying) return;
        
        // Draw the boundaries of the playfield
        Gizmos.color = Color.yellow;
        float halfWidth = playfieldWidth / 2;
        float height = 50f; // Some arbitrary height for visualization
        
        // Left boundary
        Gizmos.DrawLine(new Vector3(-halfWidth, -10f, 0), new Vector3(-halfWidth, height, 0));
        
        // Right boundary
        Gizmos.DrawLine(new Vector3(halfWidth, -10f, 0), new Vector3(halfWidth, height, 0));
        
        // Center line
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(0, -10f, 0), new Vector3(0, height, 0));
        
        // maxPath boundaries
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(maxPath, -5f, 0), new Vector3(maxPath, height, 0));
        Gizmos.DrawLine(new Vector3(-maxPath, -5f, 0), new Vector3(-maxPath, height, 0));
    }
}