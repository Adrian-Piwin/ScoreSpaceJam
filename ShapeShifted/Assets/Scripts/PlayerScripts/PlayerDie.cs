using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDie : MonoBehaviour
{
    [SerializeField] private GameManagement gameManagement;
    [SerializeField] private GameObject deathParticleSystem;
    [SerializeField] private int deathLayer;
    [SerializeField] private int boundsLayer;

    public bool isDead;
    private SpriteRenderer spriteRenderer;
    private PlayerScore playerScore;

    void OnEnable()
    {
        isDead = false;
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerScore = GetComponent<PlayerScore>();
    }

    private void Died()
    {
        if (isDead) return;

        GameObject particles = Instantiate(deathParticleSystem, transform.position, Quaternion.identity, transform.parent);
        // Tell game manager the player died
        GameObject.Find("SoundManager").GetComponent<SoundManagement>().PlayEffect("death");
        gameManagement.PlayerDied((int)playerScore.distanceTraveled, (int)playerScore.coins);
        gameObject.SetActive(false);
        Destroy(particles, 2f);
        isDead = true;
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
