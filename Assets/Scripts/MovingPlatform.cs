using UnityEditor.Callbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform PosA;
    public Transform PosB;
    public float speed;
    Vector3 targetPos;

    PlayerMovement movementController;
    Rigidbody2D rb;
    Vector3 moveDirection;

    private void Awake()
    {
        movementController = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        targetPos = PosB.position;
        DirectionCalculate();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Vector2.Distance(transform.position, PosA.position) < 0.5f)
        {
            targetPos = PosB.position;
            DirectionCalculate();
        }
        else if (Vector2.Distance(transform.position, PosB.position) < 0.5f)
        {
            targetPos = PosA.position;
            DirectionCalculate();
        }
       // transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * speed;
    }

    void DirectionCalculate(){
        moveDirection = (targetPos - transform.position).normalized;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            movementController.isOnPlatform = true;
            movementController.platformRb = rb;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            movementController.isOnPlatform = false;
        }
    }
}
   
