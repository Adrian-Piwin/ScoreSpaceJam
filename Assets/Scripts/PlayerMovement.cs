using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement Settings")]
    [SerializeField] private float speed;
    [SerializeField] private float fallMultiplier;

    [Header("Player Teleport Settings")]
    [SerializeField] private float teleportRadius;
    [SerializeField] private float teleportTimeLimit;

    [Header("References")]
    [SerializeField] private GameObject teleportCircle;

    private CircleCollider2D teleportCollider;
    private TeleportCircle teleportCircleScript;
    private Rigidbody2D body;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        teleportCollider = teleportCircle.GetComponent<CircleCollider2D>();
        teleportCircleScript = teleportCircle.GetComponent<TeleportCircle>();
        teleportCircleScript.SetRadius(teleportRadius);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        body.velocity = new Vector2(speed, body.velocity.y);

        // Better falling
        if (body.velocity.y < 0)
        {
            body.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
    }


}
