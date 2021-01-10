using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement Settings")]
    [SerializeField] private float speed;
    [SerializeField] private float fallMultiplier;

    [Header("Player Teleport Settings")]
    [SerializeField] public float teleportingCooldown;
    [SerializeField] public float teleportingSlowMotion;
    [SerializeField] private float teleportRadius;
    [SerializeField] public float teleportTimeLimit;

    [Header("References")]
    [SerializeField] public GameObject teleportDragObj;
    [SerializeField] private GameObject teleportCircle;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask teleportCircleLayer;

    // Public variables
    public bool canMove = true;

    // Private variables
    private TeleportCircle teleportCircleScript;
    private Rigidbody2D body;
    private bool teleportInputDown;
    private bool teleportInputUp;
    private bool isCircleFollowing;
    private bool forceTeleport;
    private float teleportTimer; 
    private IEnumerator forceTeleportCoroutine;

    // Public variables
    [NonSerialized] public bool isTeleporting;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();

        teleportCircleScript = teleportCircle.GetComponent<TeleportCircle>();
        teleportCircleScript.SetRadius(teleportRadius);
        forceTeleportCoroutine = ForceTeleport(teleportTimeLimit * teleportingSlowMotion);
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
        if (!canMove) return;

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
        if (!teleportInputDown || teleportTimer > Time.time || !canMove) return;

        // Check if clicked on player
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero,
            playerLayer);

        if (hit.collider != null)
        {
            teleportDragObj.SetActive(true);
            isTeleporting = true;
            isCircleFollowing = true;
            teleportCircleScript.Toggle(true);
            StartCoroutine(forceTeleportCoroutine);

            // Slow motion
            Time.timeScale = teleportingSlowMotion;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
    }

    IEnumerator ForceTeleport(float timer)
    {
        // Force teleport after certain time
        yield return new WaitForSeconds(timer);
        forceTeleport = true;
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
        if ((!isTeleporting || !teleportInputUp) && !forceTeleport) return;

        // Check if player is overlapping with wall 
        bool canTeleport = !teleportDragObj.GetComponent<PlayerDrag>().isOverlapping;
        teleportDragObj.SetActive(false);
        teleportCircleScript.Toggle(false);
        forceTeleport = false;
        isCircleFollowing = false;
        isTeleporting = false;

        StopCoroutine(forceTeleportCoroutine);

        // Cooldown
        teleportTimer = Time.time + teleportingCooldown;

        // Slow motion
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

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
