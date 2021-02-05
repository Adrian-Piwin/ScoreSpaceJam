using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 newPos = transform.position;
        newPos.x = target.position.x;

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, newPos, smoothSpeed);

        transform.position = smoothedPosition;
    }
}
