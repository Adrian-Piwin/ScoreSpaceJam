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

    // ================== DIFFICULTY ==================

    [Header("Difficulty Settings")]

    [Tooltip("A value from 0-1, indicating how difficult a platform should be; starting value")] 
    [SerializeField] private float difficultyFactorStart;

    [Tooltip("The score to reach in order for difficulty factor to be 1")]
    [SerializeField] private float difficultyMaxScore;

    [Tooltip("Max spike rate percentage on max difficulty factor")]
    [SerializeField] private float spikeRateMax;

    [Tooltip("Max wall rate percentage on max difficulty factor")]
    [SerializeField] private float wallRateMax;

    [Tooltip("Max coin rate percentage on max difficulty factor")]
    [SerializeField] private float coinRateMax;

    [Tooltip("Multiplyer for difficulty on wall")]
    [SerializeField] private float wallDifficultyMulti;

    // ================== PLATFORM SETTINGS ==================

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
    [SerializeField] private int minWallHeight;
    [SerializeField] private int maxWallHeight;

    [Header("References")] 
    [SerializeField] private Transform player;
    [SerializeField] public GameObject coinPrefab;
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
        collisionTilemap.ClearAllTiles();
        dangerTilemap.ClearAllTiles();
        clearCoins();
    }

    // ================== PLATFORM PLACEMENT ==================

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

    // ================== PLATFORM GENERATION ==================

    private void generatePlatform(Vector2Int pos)
    {
        int length = UnityEngine.Random.Range(minPlatformLength, maxPlatformLength);

        // How difficult the platform should be
        float difficultyFactor = Mathf.Lerp(difficultyFactorStart, 1, player.GetComponent<PlayerScore>().distanceTraveled / difficultyMaxScore);
        // Generate list to indicate what will spawn on platform
        List<int> platformGen = new List<int>(new int[length]);

        float spikeAmt = (spikeRateMax * difficultyFactor) * length; // 1
        float wallAmt = (wallRateMax * difficultyFactor) * length; // 2
        float coinAmt = (coinRateMax * difficultyFactor) * length; // 3

        for (int i = 0; i < length; i++) 
        {
            if ((int)coinAmt > 0)
            {
                platformGen[i] = 3;
                coinAmt--;
                continue;
            }

            if ((int)spikeAmt > 0)
            {
                platformGen[i] = 1;
                spikeAmt--;
                continue;
            }

            if ((int)wallAmt > 0)
            {
                platformGen[i] = 2;
                wallAmt--;
                continue;
            }
        }

        // Single platform
        Vector3Int newPos = Vector3Int.zero;
        for (int i = 0; i < length; i++)
        {
            // Place ground tile
            newPos = new Vector3Int(pos.x + i, pos.y, 0);
            collisionTilemap.SetTile(newPos, ground);

            // Place something on tile
            int chosen = UnityEngine.Random.Range(0, platformGen.Count);
            int objType = platformGen[chosen];
            platformGen.RemoveAt(chosen);

            switch (objType) 
            {
                case 1:
                    // Spawn spike
                    generateGroundSpike(newPos);
                    break;
                case 2:
                    // Spawn wall
                    generateWall(newPos);
                    break;
                case 3:
                    // Spawn coin
                    generateCoin(newPos);
                    break;
            }
        }

        startPos = newPos;
    }

    private void generateCoin(Vector3Int pos)
    {
        Vector3Int desiredPos;

        // Check if space above is occupied
        desiredPos = new Vector3Int(pos.x, pos.y + 1, 0);
        if (collisionTilemap.GetTile(desiredPos) == null && dangerTilemap.GetTile(desiredPos) == null)
        {
            Instantiate(coinPrefab, new Vector2(desiredPos.x + 0.5f, desiredPos.y + 0.5f), Quaternion.identity, gameObject.transform);
            return;
        }

        // Check if space under is occupied
        desiredPos = new Vector3Int(pos.x, pos.y - 1, 0);
        if (collisionTilemap.GetTile(desiredPos) == null && dangerTilemap.GetTile(desiredPos) == null)
        {
            Instantiate(coinPrefab, new Vector2(desiredPos.x + 0.5f, desiredPos.y + 0.5f), Quaternion.identity, gameObject.transform);
        }
    }

    private void generateGroundSpike(Vector3Int pos)
    {
        Vector3Int desiredPos;

        // Check if space above is occupied
        desiredPos = new Vector3Int(pos.x, pos.y + 1, 0);
        if (collisionTilemap.GetTile(desiredPos) == null && dangerTilemap.GetTile(desiredPos) == null)
        {
            dangerTilemap.SetTile(desiredPos, spikeUp);
        }

        // 25% chance of also spawning under
        if (UnityEngine.Random.Range(0, 1) < 0.75f)
            return;

        // Check if space under is occupied
        desiredPos = new Vector3Int(pos.x, pos.y - 1, 0);
        if (collisionTilemap.GetTile(desiredPos) == null && dangerTilemap.GetTile(desiredPos) == null)
        {
            dangerTilemap.SetTile(desiredPos, spikeDown);
        }
    }

    // ================== WALL GENERATION ==================

    private void generateWall(Vector3Int pos)
    {
        int wallHeight = UnityEngine.Random.Range(minWallHeight, maxWallHeight);

        // How difficult the platform should be
        float difficultyFactor = Mathf.Lerp(difficultyFactorStart, 1, player.GetComponent<PlayerScore>().distanceTraveled / difficultyMaxScore);
        // Generate list to indicate what will spawn on platform
        List<int> platformGen = new List<int>(new int[wallHeight]);

        float spikeAmt = (spikeRateMax * difficultyFactor) * wallHeight * wallDifficultyMulti; // 1
        float coinAmt = (coinRateMax * difficultyFactor) * wallHeight * wallDifficultyMulti; // 3

        for (int i = 0; i < wallHeight; i++)
        {
            if ((int)coinAmt > 0)
            {
                platformGen[i] = 3;
                coinAmt--;
                continue;
            }

            if ((int)spikeAmt > 0)
            {
                platformGen[i] = 1;
                spikeAmt--;
                continue;
            }
        }

        Vector3Int desiredPos;
        for (int i = 1; i < wallHeight; i++)
        {
            desiredPos = new Vector3Int(pos.x, pos.y + i, 0);
            // Check if occupied
            if (collisionTilemap.GetTile(desiredPos) == null && dangerTilemap.GetTile(desiredPos) == null)
            {
                // Place wall tile
                collisionTilemap.SetTile(desiredPos, ground);

                // Place something with wall
                int chosen = UnityEngine.Random.Range(0, platformGen.Count);
                int objType = platformGen[chosen];
                platformGen.RemoveAt(chosen);

                switch (objType)
                {
                    case 1:
                        // Spawn spike
                        generateWallSpike(desiredPos);
                        break;
                    case 3:
                        // Spawn coin
                        generateWallCoin(desiredPos);
                        break;
                }
            }
            else
            {
                // Stop making wall if hitting something
                break;
            }
        }
    }

    private void generateWallSpike(Vector3Int pos)
    {
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

    private void generateWallCoin(Vector3Int pos)
    {
        Vector3Int desiredPos;

        // Check if space above is occupied
        desiredPos = new Vector3Int(pos.x + 1, pos.y, 0);
        if (collisionTilemap.GetTile(desiredPos) == null && dangerTilemap.GetTile(desiredPos) == null)
        {
            Instantiate(coinPrefab, new Vector2(desiredPos.x + 0.5f, desiredPos.y + 0.5f), Quaternion.identity, gameObject.transform);
            return;
        }

        // Check if space under is occupied
        desiredPos = new Vector3Int(pos.x - 1, pos.y, 0);
        if (collisionTilemap.GetTile(desiredPos) == null && dangerTilemap.GetTile(desiredPos) == null)
        {
            Instantiate(coinPrefab, new Vector2(desiredPos.x + 0.5f, desiredPos.y + 0.5f), Quaternion.identity, gameObject.transform);
        }
    }

    // ================== OTHER ==================

    private void clearCoins() 
    {
        // Clear any pre exisiting coins
        foreach (Transform child in transform) 
        {
            if (child.gameObject.layer == LayerMask.NameToLayer("Coins"))
                Destroy(child.gameObject);
        }
    }
}
