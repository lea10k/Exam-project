using UnityEngine;

// *old wizard voice* MAKE IT RISE
public class LavaRise : MonoBehaviour
{
    public float baseSpeed = 0.5f; // starting speed
    public float growthRate = 0.1f; // how fast the speed grows

    private float timeElapsed = 0f;

    void Update()
    {
        timeElapsed += Time.deltaTime;

        // Speed grows exponentially over time
        float currentSpeed = baseSpeed * Mathf.Exp(growthRate * timeElapsed);

        // Move the lava up
        transform.position += Vector3.up * currentSpeed * Time.deltaTime;
    }
}
