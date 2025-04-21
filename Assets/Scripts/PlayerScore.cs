using UnityEngine;
using TMPro;

public class PlayerScore : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    private float highestY = 0f;

    void Start()
    {
        highestY = transform.position.y;
    }

    void Update()
    {
        // Update highest position if player moves up
        if (transform.position.y > highestY)
        {
            highestY = transform.position.y;
        }

        // Display score (rounded to int)
        scoreText.text = "Score: " + Mathf.RoundToInt(highestY);
    }
}
