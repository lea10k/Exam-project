using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public GameObject heartPrefab; // Prefab for a single heart
    public RectTransform heartsContainer; // Parent object for hearts (RectTransform for UI positioning)
    private GameObject[] hearts;

    void Start()
    {
        // Set the heartsContainer to the top-right of the canvas
        //heartsContainer.anchorMin = new Vector2(1, 1); // Top-right corner
        //heartsContainer.anchorMax = new Vector2(1, 1); // Top-right corner
        //heartsContainer.pivot = new Vector2(1, 1); // Pivot at the top-right
        //heartsContainer.anchoredPosition = new Vector2(-20, -20); // Offset from the top-right corner

        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth component not found in the scene!");
            return;
        }

        playerHealth.HealthChanged += UpdateHearts;

        // Initialize hearts
        hearts = new GameObject[playerHealth.maxHealth];
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i] = Instantiate(heartPrefab, heartsContainer);
            RectTransform heartRect = hearts[i].GetComponent<RectTransform>();

            // Adjust scale to make the hearts 2x bigger
            heartRect.localScale = new Vector3(2f, 2f, 1f);

            // Space hearts horizontally
            heartRect.anchoredPosition = new Vector2(-i * 40, 0); // Space hearts to the left
        }
    }

    private void UpdateHearts(int currentHealth)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].SetActive(i < currentHealth);
        }
    }
}