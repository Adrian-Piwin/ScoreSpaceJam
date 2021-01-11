using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class SpikeWallScript : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float maxDistance;
    [SerializeField] private float speed;
    public bool canMove;

    private Vector3 startingPos;

    void Start()
    {
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!canMove) return;

        if (Vector2.Distance(transform.position, player.position) > maxDistance)
            transform.position = new Vector3(player.position.x - maxDistance, transform.position.y, 0);

        transform.position = transform.position + (transform.right * (speed * Time.deltaTime));
    }

    public void reset()
    {
        transform.position = startingPos;
    }
}
