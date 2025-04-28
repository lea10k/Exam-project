using UnityEngine;

public class Paralex : MonoBehaviour
{
    //private float height;
    private float startPos;
    private GameObject cam;
    [SerializeField] private float parallaxEffect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = GameObject.Find("CinemachineCamera");
        startPos = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = (cam.transform.position.y * parallaxEffect);
        transform.position = new Vector3(transform.position.x, startPos + distance, transform.position.z);
    }
}
