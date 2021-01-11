using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

public class LevelGeneration : MonoBehaviour
{
    [Header("Settings")] 
    [SerializeField] private Vector3Int startPos;
    [SerializeField] private float spawnDistance;

    [Header("Spawn Settings")]
    [SerializeField] public bool canGenerateWallSpikes = true;
    [SerializeField] public bool canGenerateGroundSpikes = true;
    [SerializeField] public bool canGenerateWalls = true;

    [Header("Multi Level")]
    [SerializeField] private float multiLevelRate;
    [SerializeField] private int minMultiLevel;
    [SerializeField] private int maxMultiLevel;

    [Header("Platform Length")]
    [SerializeField] private int minPlatformLength;
    [SerializeField] private int maxPlatformLength;

    [Header("Platform Distance")]
    [SerializeField] private int minPlatformDistance;
    [SerializeField] private int maxPlatformDistance;

    [Header("Platform Height Range")]
    [SerializeField] private int minPlatformHeight;
    [SerializeField] private int maxPlatformHeight;

    [Header("Platform Wall Length")]
    [SerializeField] public float wallRate;
    [SerializeField] private int minWallHeight;
    [SerializeField] private int maxWallHeight;

    [Header("Platform Spikes")] 
    [SerializeField] public float spikeWallRate;
    [SerializeField] public float spikeGroundRate;

    [Header("References")] 
    [SerializeField] private Transform player;
    [SerializeField] private Tilemap collisionTilemap;
    [SerializeField] private Tilemap dangerTilemap;
    [SerializeField] private Tile ground;
    [SerializeField] private Tile spikeLeft;
    [SerializeField] private Tile spikeRight;
    [SerializeField] private Tile spikeUp;
    [SerializeField] private Tile spikeDown;

    private Vector3Int startGenerationPos;

    // Start is called before the first frame update
    void Start()
    {
        startGenerationPos = startPos;
    }

    // Update is called once per frame
    void Update()
    {
        placePlatform();
    }

    public void resetGeneration()
    {
        startPos = startGenerationPos;
    }

    private void placePlatform()
    {
        // Check if player is within spawn distance
        if (Vector2.Distance(new Vector2(startPos.x, startPos.y), player.position) < spawnDistance)
        {
            int height = UnityEngine.Random.Range(-maxPlatformDistance, maxPlatformDistance);
            int width = UnityEngine.Random.Range(minPlatformDistance, maxPlatformDistance);
            Vector2Int spawnPos = new Vector2Int(startPos.x + width, Mathf.Clamp(startPos.y + height, minPlatformHeight, maxPlatformHeight));

            // Generate platform
            generatePlatform(spawnPos);
        }
    }

    private void generatePlatform(Vector2Int pos)
    {
        int length = UnityEngine.Random.Range(minPlatformLength, maxPlatformLength);

        // Single platform
        Vector3Int newPos = Vector3Int.zero;
        for (int i = 0; i < length; i++)
        {
            newPos = new Vector3Int(pos.x + i, pos.y, 0);
            collisionTilemap.SetTile(newPos, ground);

            generateGroundSpike(newPos);
            generateWall(newPos);
        }

        // Multi Platform
        if (UnityEngine.Random.Range(0f, 1f) < multiLevelRate)
        {
            // Get new height for multi level platform
            int newHeight = pos.y > maxPlatformHeight / 2 ? -1 : 1;
            newHeight *= Mathf.Clamp(UnityEngine.Random.Range(minMultiLevel, maxMultiLevel), minPlatformHeight, maxPlatformHeight);

            // Generate platform
            for (int i = 0; i < length; i++)
            {
                newPos = new Vector3Int(pos.x + i, newHeight, 0);
                collisionTilemap.SetTile(newPos, ground);

                generateGroundSpike(newPos);
                generateWall(newPos);
            }
        }

        startPos = newPos;
    }

    private void generateGroundSpike(Vector3Int pos)
    {
        // Spike rate
        if (UnityEngine.Random.Range(0f, 1f) > spikeGroundRate || !canGenerateGroundSpikes) return;

        Vector3Int desiredPos;

        // Check if space above is occupied
        desiredPos = new Vector3Int(pos.x, pos.y + 1, 0);
        if (collisionTilemap.GetTile(desiredPos) == null && dangerTilemap.GetTile(desiredPos) == null)
        {
            dangerTilemap.SetTile(desiredPos, spikeUp);
            return;
        }

        // Check if space under is occupied
        desiredPos = new Vector3Int(pos.x, pos.y - 1, 0);
        if (collisionTilemap.GetTile(desiredPos) == null && dangerTilemap.GetTile(desiredPos) == null)
        {
            dangerTilemap.SetTile(desiredPos, spikeDown);
        }
    }

    private void generateWallSpike(Vector3Int pos)
    {
        // Spike rate
        if (UnityEngine.Random.Range(0f, 1f) > spikeWallRate || !canGenerateWallSpikes) return;

        Vector3Int desiredPos;

        // Check if space above is occupied
        desiredPos = new Vector3Int(pos.x + 1, pos.y, 0);
        if (collisionTilemap.GetTile(desiredPos) == null && dangerTilemap.GetTile(desiredPos) == null)
        {
            dangerTilemap.SetTile(desiredPos, spikeRight);
            return;
        }

        // Check if space under is occupied
        desiredPos = new Vector3Int(pos.x - 1, pos.y, 0);
        if (collisionTilemap.GetTile(desiredPos) == null && dangerTilemap.GetTile(desiredPos) == null)
        {
            dangerTilemap.SetTile(desiredPos, spikeLeft);
        }
    }

    private void generateWall(Vector3Int pos)
    {
        if (UnityEngine.Random.Range(0f, 1f) > wallRate || !canGenerateWalls) return;

        int wallHeight = UnityEngine.Random.Range(minWallHeight, maxWallHeight);

        Vector3Int desiredPos;
        for (int i = 1; i < wallHeight; i++)
        {
            desiredPos = new Vector3Int(pos.x, pos.y + i, 0);
            // Check if occupied
            if (collisionTilemap.GetTile(desiredPos) == null && dangerTilemap.GetTile(desiredPos) == null)
            {
                collisionTilemap.SetTile(desiredPos, ground);
                generateWallSpike(desiredPos);
            }
            else
            {
                // Stop making wall if hitting something
                break;
            }
        }
    }
}
