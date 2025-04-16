using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    public GameObject[] platformPrefabs;
    public float verticalSpacing = 2.5f;
    public float horizontalRange = 3f;
    public Transform player;
    private float lastY;

    void Start() {
        lastY = 0f;
        for (int i = 0; i < 10; i++) {
            GeneratePlatform();
        }
    }

    void Update() {
        // Überprüfen, ob der Spieler nahe an der höchsten Plattform ist
        if (player.position.y + 10f > lastY) {
            GeneratePlatform();
        }
    }

    void GeneratePlatform() {
        float x = Random.Range(-horizontalRange, horizontalRange);
        lastY += verticalSpacing;
        Vector2 spawnPos = new Vector2(x, lastY);
        int prefabIndex = Random.Range(0, platformPrefabs.Length);
        Instantiate(platformPrefabs[prefabIndex], spawnPos, Quaternion.identity);
    }

}
