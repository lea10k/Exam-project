using UnityEditor.Callbacks;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform PosA;
    public Transform PosB;
    public float speed;
    Vector3 targetPos;

    private void Start()
    {
        targetPos = PosB.position;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Vector2.Distance(transform.position, PosA.position) < 0.5f)
        {
            targetPos = PosB.position;
        }
        else if (Vector2.Distance(transform.position, PosB.position) < 0.5f)
        {
            targetPos = PosA.position;
        }
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.parent = this.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.parent = null;
        }
    }
}
   
