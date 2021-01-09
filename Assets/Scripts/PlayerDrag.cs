using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDrag : MonoBehaviour
{
    public bool isOverlapping;

    void OnTriggerEnter2D(Collider2D other)
    {
        isOverlapping = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        isOverlapping = false;
    }
}
