using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private float maxDespawnDistance = 15f;
    private Transform cam;

    void Start()
    {
        cam = GameObject.Find("Main Camera").transform;
    }

    // Check for coins
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Player" && other is BoxCollider2D)
        {
            other.GetComponent<PlayerScore>().PickupCoin();
            GameObject.Find("SoundManager").GetComponent<SoundManagement>().PlayEffect("coin");
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (transform.position.x < cam.position.x - maxDespawnDistance)
            Destroy(gameObject);
    }

}
