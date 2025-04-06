using UnityEngine;

public class InfiniteBackground : MonoBehaviour
{
    private float height;
    private GameObject cam;

    void Start()
    {
        cam = GameObject.Find("CinemachineCamera");
        height = GetComponent<SpriteRenderer>().bounds.size.y;
    }

    void Update()
    {
        // Wenn Kamera über dem aktuellen Sprite ist → verschiebe Sprite nach oben
        if (cam.transform.position.y > transform.position.y + height)
        {
            transform.position += new Vector3(0f, height * 2f, 0f);
        }
    }
}
