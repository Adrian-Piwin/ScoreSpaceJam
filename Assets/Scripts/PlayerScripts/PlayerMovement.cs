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
    [SerializeField] private float maxDownVel;

    [Header("Player Teleport Settings")]
    [SerializeField] public float teleportingCooldown;
    [SerializeField] public float teleportingSlowMotion;
    [SerializeField] private float teleportRadius;
    [SerializeField] public float teleportTimeLimit;

    [Header("References")]
    [SerializeField] public GameObject teleportDragObj;
    [SerializeField] private GameObject teleportCircle;
    [SerializeField] private LayerMask teleportCircleLayer;

    // Public variables
    public bool canMove = true;
    public bool canTeleport = true;

    // Private variables
    private TeleportCircle teleportCircleScript;
    private Rigidbody2D body;
    private PlayerDie playerDie;
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
        playerDie = GetComponent<PlayerDie>();

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
        if (!canMove)
        {
            body.velocity = Vector2.zero;
            return;
        }

        // Player consistent movement
        body.velocity = new Vector2(speed, body.velocity.y);

        // Better falling
        if (body.velocity.y < 0)
        {
            body.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }

        // Set max falling velocity
        if (body.velocity.y < maxDownVel)
            body.velocity = new Vector2(body.velocity.x, maxDownVel);
    }

    // =========================== Teleporting ===========================

    private void TeleportCircleFollow()
    {
        if (!isCircleFollowing) return;
        teleportCircle.transform.position = transform.position;
    }

    private void TeleportStart()
    {
        if (!teleportInputDown || teleportTimer > Time.time || playerDie.isDead || !canTeleport) return;

        // Check if clicked on player
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit == false) return;

        if (hit.transform.gameObject.name == "Player")
        {
            teleportDragObj.SetActive(true);
            isTeleporting = true;
            isCircleFollowing = true;
            teleportCircle.transform.position = transform.position;
            teleportCircleScript.Toggle(true);

            forceTeleportCoroutine = ForceTeleport(teleportTimeLimit * teleportingSlowMotion);
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

        StopCoroutine(forceTeleportCoroutine);

        // Check if player is overlapping with wall 
        bool canTeleport = !teleportDragObj.GetComponent<PlayerDrag>().isOverlapping;
        teleportDragObj.SetActive(false);
        teleportCircleScript.Toggle(false);
        forceTeleport = false;
        isCircleFollowing = false;
        isTeleporting = false;

        // Cooldown
        teleportTimer = Time.time + teleportingCooldown;

        // Slow motion
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        if (canTeleport && canMove)
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

    // Check for bounce
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == 0 && body.velocity.magnitude > 6f && !isTeleporting)
        {
            // bounce!
            GameObject.Find("SoundManager").GetComponent<SoundManagement>().PlayEffect("bounce");
        }
    }

    void OnDisable()
    {
        // On player death

        // Stop teleport stuff
        StopCoroutine(forceTeleportCoroutine);

        teleportDragObj.SetActive(false);
        teleportCircleScript.Toggle(false);
        forceTeleport = false;
        isCircleFollowing = false;
        isTeleporting = false;

        // Slow motion
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }
}
