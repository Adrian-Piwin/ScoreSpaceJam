using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreUI;

    private Vector2 startPos;
    public float distanceTraveled;
    public float coins;

    void OnEnable()
    {
        distanceTraveled = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!scoreUI.enabled) return;

        if (distanceTraveled < Vector2.Distance(startPos, transform.position))
            distanceTraveled = Mathf.Floor(Vector2.Distance(startPos, transform.position));

        scoreUI.text = distanceTraveled + " M";
    }

    public void PickupCoin()
    {
        coins++;
    }
}
