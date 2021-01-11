using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class SpikeWallScript : MonoBehaviour
{
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

        transform.position = transform.position + (transform.right * (speed * Time.deltaTime));
    }

    public void reset()
    {
        transform.position = startingPos;
    }
}
