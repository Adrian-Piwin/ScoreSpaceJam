using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
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
    [SerializeField] public bool canGenerateCoins = true;

    [Header("Multi Level")]
    [SerializeField] private float multiLevelRate;

    [Header("Platform Length")]
    [SerializeField] private int minPlatformLength;
    [SerializeField] private int maxPlatformLength;

    [Header("Platform Distance")]
    [SerializeField] private int minPlatformDistanceWidth;
    [SerializeField] private int maxPlatformDistanceWidth;

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

    [Header("Coins")]
    [SerializeField] public GameObject coinPrefab;
    [SerializeField] public float coinRate;

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
        Vector3Int curStartPos = startPos;

        // Check if player is within spawn distance
        if (Vector2.Distance(new Vector2(curStartPos.x, curStartPos.y), player.position) < spawnDistance)
        {
            Vector2Int spawnPos;

            if (UnityEngine.Random.Range(0f, 1f) < multiLevelRate)
            {
                int width = UnityEngine.Random.Range(minPlatformDistanceWidth, maxPlatformDistanceWidth);

                // Generate platform on top and bottom half of screen
                spawnPos = new Vector2Int(curStartPos.x + width,
                    UnityEngine.Random.Range(((maxPlatformHeight - minPlatformHeight) / 2) + minPlatformHeight, maxPlatformHeight) + 1);

                generatePlatform(spawnPos);

                spawnPos = new Vector2Int(curStartPos.x + width,
                    UnityEngine.Random.Range(minPlatformHeight, ((maxPlatformHeight - minPlatformHeight) / 2) + minPlatformHeight) - 1);

                generatePlatform(spawnPos);
            }
            else
            {
                // Generate platform in middle of screen
                spawnPos = new Vector2Int(curStartPos.x + UnityEngine.Random.Range(minPlatformDistanceWidth, maxPlatformDistanceWidth),
                    UnityEngine.Random.Range(
                        (int)((((maxPlatformHeight - minPlatformHeight) * 0.33) + minPlatformHeight) - 1),
                    (int)(((maxPlatformHeight - minPlatformHeight) * 0.66) + minPlatformHeight) + 1)
                    );

                generatePlatform(spawnPos);
            }
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

            bool didGenerate = false;
            didGenerate = generateCoin(newPos);
            if (didGenerate) continue;
            didGenerate = generateGroundSpike(newPos);
            if (didGenerate) continue;
            generateWall(newPos);
        }

        startPos = newPos;
    }

    private bool generateCoin(Vector3Int pos)
    {
        // Spike rate
        if (UnityEngine.Random.Range(0f, 1f) > coinRate || !canGenerateCoins) return false;

        Vector3Int desiredPos;

        // Check if space above is occupied
        desiredPos = new Vector3Int(pos.x, pos.y + 1, 0);
        if (collisionTilemap.GetTile(desiredPos) == null && dangerTilemap.GetTile(desiredPos) == null)
        {
            Instantiate(coinPrefab, new Vector3(desiredPos.x + 0.5f, desiredPos.y + 0.5f, 0), Quaternion.identity, gameObject.transform);
            return true;
        }

        // Check if space under is occupied
        desiredPos = new Vector3Int(pos.x, pos.y - 1, 0);
        if (collisionTilemap.GetTile(desiredPos) == null && dangerTilemap.GetTile(desiredPos) == null)
        {
            Instantiate(coinPrefab, new Vector3(desiredPos.x - 0.5f, desiredPos.y - 0.5f, 0), Quaternion.identity, gameObject.transform);
        }

        return true;
    }

    private bool generateGroundSpike(Vector3Int pos)
    {
        // Spike rate
        if (UnityEngine.Random.Range(0f, 1f) > spikeGroundRate || !canGenerateGroundSpikes) return false;

        Vector3Int desiredPos;

        // Check if space above is occupied
        desiredPos = new Vector3Int(pos.x, pos.y + 1, 0);
        if (collisionTilemap.GetTile(desiredPos) == null && dangerTilemap.GetTile(desiredPos) == null)
        {
            dangerTilemap.SetTile(desiredPos, spikeUp);
            return true;
        }

        // Check if space under is occupied
        desiredPos = new Vector3Int(pos.x, pos.y - 1, 0);
        if (collisionTilemap.GetTile(desiredPos) == null && dangerTilemap.GetTile(desiredPos) == null)
        {
            dangerTilemap.SetTile(desiredPos, spikeDown);
        }

        return true;
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

    private bool generateWall(Vector3Int pos)
    {
        if (UnityEngine.Random.Range(0f, 1f) > wallRate || !canGenerateWalls) return false;

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

        return true;
    }
}
