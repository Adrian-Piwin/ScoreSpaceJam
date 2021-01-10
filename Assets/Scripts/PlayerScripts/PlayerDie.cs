using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDie : MonoBehaviour
{
    [SerializeField] private GameObject deathParticleSystem;
    [SerializeField] private int deathLayer;
    [SerializeField] private int boundsLayer;

    public bool isDead;
    private SpriteRenderer spriteRenderer;
    private PlayerMovement playerMovement;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Died()
    {
        if (isDead) return;

        GameObject particles = Instantiate(deathParticleSystem, transform.position, Quaternion.identity, transform.parent);
        spriteRenderer.enabled = false;
        playerMovement.canMove = false;
        Destroy(particles, 2f);
        isDead = true;
    }

    private void Respawn()
    {
        transform.position = Vector2.zero;
        spriteRenderer.enabled = true;
        playerMovement.canMove = true;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.gameObject.layer == deathLayer)
            Died();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == boundsLayer)
            Died();
    }
}
