using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDie : MonoBehaviour
{
    [SerializeField] private GameObject deathParticleSystem;
    [SerializeField] private int deathLayer;

    private void died()
    {
        GameObject particles = Instantiate(deathParticleSystem, transform.position, Quaternion.identity, transform.parent);
        Destroy(gameObject, 0.05f);
        Destroy(particles, 2f);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.gameObject.layer == deathLayer)
        {
            died();
        }
    }
}
