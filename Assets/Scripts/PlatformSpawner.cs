// PlatformSpawner.cs
// Spawns random platforms for an endless vertical 2D platformer game

using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [Tooltip("Array of platform prefabs (0 = static, 1 = disappearing, 2 = falling, 3-5 = moving)")]
    public GameObject[] platformPrefabs;

    [Header("Playfield")]
    public Transform player;
    [Tooltip("Gesamte Spielfeldbreite")]
    public float playfieldWidth = 26f;
    [Tooltip("Maximaler horizontaler Bewegungsbereich (von -maxPath bis +maxPath)")]
    public float maxPath = 12f; 
    [Tooltip("Automatisch die Breite der Plattform ermitteln? (Empfohlen)")]
    public bool autoDetectPlatformWidth = true;
    [Tooltip("Manuelle Plattformbreite (nur verwendet wenn autoDetect = false)")]
    public float manualPlatformWidth = 3f;

    [Header("Spacing Rules")]
    public float verticalSpacing = 2.5f;
    public float horizontalDistance = 3f;

    [Header("Generation Settings")]
    [Tooltip("How far ahead of player to generate platforms")]
    public float generationLookAhead = 10f;
    [Tooltip("Maximum consecutive platforms in same direction")]
    public int maxConsecutiveDirections = 3;

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
        MovingHorizontalRight = 3,
        MovingHorizontalLeft = 4,
        MovingVertical = 5
    }

    // Internal tracking variables
    private float lastSpawnedY = 0f;
    private float lastSpawnedX = 0f; // Start at the center (x=0)
    private bool wasLastPlatformMoving = false;
    private int consecutiveLeftMoves = 0;
    private int consecutiveRightMoves = 0;
    private int totalPlatformsSpawned = 0;
    private int fallingPlatformsInGroup = 0;
    private float platformWidth;

    void Start()
    {
        InitializePlatformWidth();
        ValidatePlayfieldSettings();
        
        // Spawn initial platform and setup starting state
        SpawnInitialPlatform();
        
        // Generate initial platforms
        for (int i = 0; i < 10; i++)
        {
            GenerateNextPlatform();
        }
    }

    private void InitializePlatformWidth()
    {
        if (autoDetectPlatformWidth && platformPrefabs.Length > 0)
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
                // Try to find width from any SpriteRenderer or MeshRenderer in children
                Renderer[] renderers = platformPrefabs[0].GetComponentsInChildren<Renderer>();
                if (renderers.Length > 0)
                {
                    float maxWidth = 0f;
                    foreach (Renderer r in renderers)
                    {
                        if (r.bounds.size.x > maxWidth)
                            maxWidth = r.bounds.size.x;
                    }
                    platformWidth = maxWidth;
                    Debug.Log($"Auto-detected platform width from children: {platformWidth}");
                }
                else
                {
                    // Fallback to manual width
                    platformWidth = manualPlatformWidth;
                    Debug.LogWarning($"Could not detect platform width. Using manual value: {platformWidth}");
                }
            }
        }
        else
        {
            // Use the manual platform width
            platformWidth = manualPlatformWidth;
        }
        
        // Ensure we have a minimum width to avoid division by zero
        if (platformWidth <= 0.1f)
        {
            platformWidth = 1f;
            Debug.LogWarning("Platform width too small. Using default value: 1");
        }
    }

    private void ValidatePlayfieldSettings()
    {
        // Make sure maxPath doesn't exceed half the playfield width
        float halfPlayfieldWidth = playfieldWidth / 2;
        
        if (maxPath > halfPlayfieldWidth)
        {
            Debug.LogWarning($"maxPath ({maxPath}) exceeds half of playfield width ({halfPlayfieldWidth}). Adjusting to fit.");
            maxPath = halfPlayfieldWidth;
        }
        
        // Ensure maxPath accommodates platform width
        float requiredSpace = platformWidth / 2;
        if (maxPath + requiredSpace > halfPlayfieldWidth)
        {
            float newMaxPath = halfPlayfieldWidth - requiredSpace;
            Debug.LogWarning($"Reducing maxPath to {newMaxPath} to ensure platforms don't extend beyond playfield.");
            maxPath = newMaxPath;
        }
        
        // Make sure we have enough horizontal distance for movement
        if (horizontalDistance > maxPath)
        {
            Debug.LogWarning($"horizontalDistance ({horizontalDistance}) is greater than maxPath ({maxPath}). Adjusting horizontalDistance.");
            horizontalDistance = maxPath * 0.75f;
        }
        
        Debug.Log($"Using maxPath: {maxPath}, effective platform movement range: -{maxPath} to +{maxPath}");
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
        if (totalPlatformsSpawned > 0 && totalPlatformsSpawned % 5 == 0)
        {
            // Choose randomly between horizontal movers or vertical mover
            int[] movingOptions = { 
                (int)PlatformType.MovingHorizontalRight, 
                (int)PlatformType.MovingHorizontalLeft, 
                (int)PlatformType.MovingVertical 
            };
            
            return movingOptions[Random.Range(0, movingOptions.Length)];
        }
        else
        {
            // For normal platforms, choose between static, disappearing, and falling
            bool canSpawnFalling = fallingPlatformsInGroup < 1;
            
            if (canSpawnFalling)
            {
                // Can spawn any normal platform type
                return Random.Range(0, 3); // 0, 1, or 2
            }
            else
            {
                // Can only spawn static or disappearing
                return Random.Range(0, 2); // 0 or 1
            }
        }
    }

    private Vector2 CalculateSpawnPosition(int platformIndex)
    {
        // Calculate horizontal position based on movement constraints
        float spawnX = CalculateHorizontalPosition();
        
        // Calculate vertical position (always above last platform)
        float spawnY = lastSpawnedY + verticalSpacing;
        
        // Apply platform-specific position adjustments
        Vector2 spawnPosition = AdjustPositionForPlatformType(platformIndex, spawnX, spawnY);
        
        return spawnPosition;
    }
    
    private float CalculateHorizontalPosition()
    {
        float nextX = lastSpawnedX;
        bool canMoveLeft = consecutiveLeftMoves < maxConsecutiveDirections;
        bool canMoveRight = consecutiveRightMoves < maxConsecutiveDirections;
        
        // Safe distance from edge (to avoid issues with moving platforms)
        float safeDistance = horizontalDistance * 0.5f;
        
        // Handle boundary constraints
        if (lastSpawnedX >= maxPath - safeDistance)
        {
            // At or approaching right edge, must move left
            nextX = lastSpawnedX - horizontalDistance;
            consecutiveLeftMoves++;
            consecutiveRightMoves = 0;
        }
        else if (lastSpawnedX <= -maxPath + safeDistance)
        {
            // At or approaching left edge, must move right
            nextX = lastSpawnedX + horizontalDistance;
            consecutiveRightMoves++;
            consecutiveLeftMoves = 0;
        }
        else
        {
            // Not at edge, determine direction based on constraints
            if (canMoveLeft && canMoveRight)
            {
                // Can go either way - random choice
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
                // Only left allowed
                nextX = lastSpawnedX - horizontalDistance;
                consecutiveLeftMoves++;
                consecutiveRightMoves = 0;
            }
            else
            {
                // Only right allowed
                nextX = lastSpawnedX + horizontalDistance;
                consecutiveRightMoves++;
                consecutiveLeftMoves = 0;
            }
        }
        
        // Clamp to ensure we stay within boundaries
        nextX = Mathf.Clamp(nextX, -maxPath, maxPath);
        
        return nextX;
    }
    
    private Vector2 AdjustPositionForPlatformType(int platformIndex, float x, float y)
    {
        float adjustedX = x;
        float adjustedY = y;
        
        // Calculate safe offsets that won't push platform outside boundaries
        float maxRightOffset = (playfieldWidth / 2) - adjustedX - platformWidth/2;
        float maxLeftOffset = adjustedX - (-playfieldWidth / 2 + platformWidth/2);
        
        // Apply platform-specific position adjustments
        switch (platformIndex)
        {
            case (int)PlatformType.MovingHorizontalRight:
                // Use a smaller offset for moving platforms to keep them in bounds during movement
                float rightOffset = Mathf.Min(2f, maxRightOffset * 0.5f);
                if (rightOffset > 0)
                    adjustedX += rightOffset;
                break;
                
            case (int)PlatformType.MovingHorizontalLeft:
                float leftOffset = Mathf.Min(2f, maxLeftOffset * 0.5f);
                if (leftOffset > 0)
                    adjustedX -= leftOffset;
                break;
                
            case (int)PlatformType.MovingVertical:
                adjustedY += 1f; // Start slightly higher
                break;
        }
        
        // One final check to make sure we're within the actual playfield
        float halfPlayfieldWidth = playfieldWidth / 2;
        adjustedX = Mathf.Clamp(adjustedX, -halfPlayfieldWidth + platformWidth/2, halfPlayfieldWidth - platformWidth/2);
        
        return new Vector2(adjustedX, adjustedY);
    }
    
    private void UpdateTrackingVariables(int platformIndex, Vector2 spawnPosition)
    {
        // Update position tracking
        lastSpawnedX = spawnPosition.x;
        lastSpawnedY = spawnPosition.y;
        
        // Update platform type tracking
        wasLastPlatformMoving = IsMovingPlatform(platformIndex);
        
        // Update falling platform counter
        if (platformIndex == (int)PlatformType.Falling)
        {
            fallingPlatformsInGroup++;
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
        return platformIndex >= (int)PlatformType.MovingHorizontalRight && 
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
        Gizmos.DrawLine(new Vector3(maxPath, -5f, 0), new Vector3(maxPath, 5f, 0));
        Gizmos.DrawLine(new Vector3(-maxPath, -5f, 0), new Vector3(-maxPath, 5f, 0));
    }
}