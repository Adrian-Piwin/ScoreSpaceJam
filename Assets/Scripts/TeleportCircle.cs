using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportCircle : MonoBehaviour
{
    [SerializeField] private float lerpSpeed;
    private float radius;
    private bool isEnabled;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        lerpRadius();
    }

    public void SetRadius(float radius)
    {
        this.radius = radius;
    }

    public void Toggle(bool toggle)
    {
        isEnabled = toggle;
    }

    private void lerpRadius()
    {
        // Lerp to radius
        if (Mathf.Abs(radius - transform.localScale.x) > 0.02f && isEnabled)
            transform.localScale = new Vector2( Mathf.Lerp(transform.localScale.x, radius, lerpSpeed), Mathf.Lerp(transform.localScale.y, radius, lerpSpeed));

        // Lerp to zero
        if (Mathf.Abs(radius - transform.localScale.x) > 0.02f && !isEnabled)
            transform.localScale = new Vector2(Mathf.Lerp(transform.localScale.x, 0, lerpSpeed), Mathf.Lerp(transform.localScale.y, 0, lerpSpeed));
    }
}
