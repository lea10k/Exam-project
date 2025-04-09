using UnityEngine;

public class DisappearingPlattform : MonoBehaviour
{
    public float timeToTogglePlatform = 2f;
    private float currentTime = 0f;
    private bool platformVisible = true;

    void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime >= timeToTogglePlatform)
        {
            currentTime = 0f;
            TogglePlatform();
        }
    }

    void TogglePlatform()
    {
        platformVisible = !platformVisible;

        GetComponent<SpriteRenderer>().enabled = platformVisible;
        GetComponent<BoxCollider2D>().enabled = platformVisible;
    }
}
