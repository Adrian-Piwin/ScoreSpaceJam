using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreUI;

    private Vector2 startPos;
    private float distanceTravelled;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (distanceTravelled < Vector2.Distance(startPos, transform.position))
            distanceTravelled = Mathf.Floor(Vector2.Distance(startPos, transform.position));

        scoreUI.text = distanceTravelled + " M";
    }
}
