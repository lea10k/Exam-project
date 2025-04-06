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
        //height = gameObject.GetComponent<SpriteRenderer>().bounds.size.y;
    }

    // Update is called once per frame
    void Update()
    {
        // float temp = (cam.transform.position.y * (1 - parallaxEffect));
        float distance = (cam.transform.position.y * parallaxEffect);
        transform.position = new Vector3(transform.position.x, startPos + distance, transform.position.z);

        /*if(temp > startPos + height)
        {
            startPos += height;
        }
        else if(temp < startPos - height)
        {
            startPos -= height;
        }*/
    }
}
