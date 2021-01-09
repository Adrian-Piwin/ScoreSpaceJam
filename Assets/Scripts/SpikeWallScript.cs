using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class SpikeWallScript : MonoBehaviour
{
    [SerializeField] private float speed;
    public bool canMove;

    // Update is called once per frame
    void Update()
    {
        if (!canMove) return;

        transform.position = transform.position + (transform.right * (speed * Time.deltaTime));
    }
}
