using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

public class LevelGeneration : MonoBehaviour
{
    [Header("Settings")] 
    [SerializeField] private Vector3Int startPos;
    [SerializeField] private float spawnDistance;

    [SerializeField] private int minPlatformLength;
    [SerializeField] private int maxPlatformLength;

    [SerializeField] private int minPlatformDistance;
    [SerializeField] private int maxPlatformDistance;

    [SerializeField] private int minPlatformHeight;
    [SerializeField] private int maxPlatformHeight;

    [SerializeField] private int minWallLength;
    [SerializeField] private int maxWallLength;

    [SerializeField] private int minSpikes;
    [SerializeField] private int maxSpikes;

    [Header("References")] 
    [SerializeField] private Transform player;
    [SerializeField] private Tilemap collisionTilemap;
    [SerializeField] private Tilemap dangerTilemap;
    [SerializeField] private Tile ground;
    [SerializeField] private Tile spikeLeft;
    [SerializeField] private Tile spikeRight;
    [SerializeField] private Tile spikeUp;
    [SerializeField] private Tile spikeDown;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        placePlatform();
    }

    private void placePlatform()
    {
        // Check if player is within spawn distance
        if (Vector2.Distance(new Vector2(startPos.x, startPos.y), player.position) < spawnDistance)
        {
            int height = UnityEngine.Random.Range(-maxPlatformDistance, maxPlatformDistance);
            int width = UnityEngine.Random.Range(minPlatformDistance, maxPlatformDistance);
            Vector2Int spawnPos = new Vector2Int(startPos.x + width, Mathf.Clamp(startPos.y + height, minPlatformHeight, maxPlatformHeight));

            generatePlatform(spawnPos);
        }
    }

    private void generatePlatform(Vector2Int pos)
    {
        int length = UnityEngine.Random.Range(minPlatformLength, maxPlatformLength);

        Vector3Int newPos = Vector3Int.zero;
        for (int i = 0; i < length; i++)
        {
            newPos = new Vector3Int(pos.x + i, pos.y, 0);
            collisionTilemap.SetTile(newPos, ground);
        }

        startPos = newPos;
    }
}
