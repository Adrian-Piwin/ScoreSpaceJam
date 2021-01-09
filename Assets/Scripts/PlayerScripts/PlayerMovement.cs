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
    [SerializeField] private GameObject teleportDragObj;
    [SerializeField] private GameObject teleportCircle;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask teleportCircleLayer;

    private TeleportCircle teleportCircleScript;
    private Rigidbody2D body;
    private bool teleportInputDown;
    private bool teleportInputUp;
    private bool isTeleporting;
    private bool isCircleFollowing;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();

        teleportCircleScript = teleportCircle.GetComponent<TeleportCircle>();
        teleportCircleScript.SetRadius(teleportRadius);
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        TeleportCircleFollow();
        TeleportStart();
        TeleportDrag();
        TeleportEnd();
    }

    void FixedUpdate()
    {
        Movement();
    }

    private void GetInput()
    {
        // Teleport input
        teleportInputDown = Input.GetButtonDown("Fire1");
        teleportInputUp = Input.GetButtonUp("Fire1");
    }

    private void Movement()
    {
        // Player consistent movement
        body.velocity = new Vector2(speed, body.velocity.y);

        // Better falling
        if (body.velocity.y < 0)
        {
            body.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
    }

    // =========================== Teleporting ===========================

    private void TeleportCircleFollow()
    {
        if (!isCircleFollowing) return;
        teleportCircle.transform.position = transform.position;
    }

    private void TeleportStart()
    {
        if (!teleportInputDown) return;

        // Check if clicked on player
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero,
            playerLayer);

        if (hit.collider != null)
        {
            teleportDragObj.SetActive(true);
            isTeleporting = true;
            isCircleFollowing = true;
            teleportCircleScript.Toggle(true);
        }
    }

    private void TeleportDrag()
    {
        if (!isTeleporting) return;

        // Drag where player wants to teleport
        Vector2 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        teleportDragObj.transform.position = newPos;
    }

    private void TeleportEnd()
    {
        if (!isTeleporting || !teleportInputUp) return;

        // Check if player is overlapping with wall 
        bool canTeleport = !teleportDragObj.GetComponent<PlayerDrag>().isOverlapping;
        teleportDragObj.SetActive(false);
        teleportCircleScript.Toggle(false);
        isCircleFollowing = false;
        isTeleporting = false;

        if (canTeleport)
            Teleport();
    }

    private void Teleport()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, teleportCircleLayer);

        //Check if player is teleporting within radius
        if (hit.collider != null)
        {
            transform.position = teleportDragObj.transform.position;
        }
    }
}
