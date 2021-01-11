using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManagement : MonoBehaviour
{
    [Header("Other")]
    [SerializeField] private Vector3 spawnPos;
    [SerializeField] private Camera cam;

    [Header("Player")]
    [SerializeField] private GameObject player;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerDie playerDie;
    [SerializeField] private PlayerScore playerScore;

    [Header("UI")]
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private TextMeshProUGUI scoreUI;
    [SerializeField] private TextMeshProUGUI gameOverScoreUI;
    [SerializeField] private TextMeshProUGUI gameOverHighScoreUI;
    [SerializeField] private TextMeshProUGUI gameOverNewHighScoreUI;

    [Header("Level")]
    [SerializeField] private LevelGeneration levelGeneration;
    [SerializeField] private SpikeWallScript spikeWall;
    [SerializeField] private Tilemap generatedCollisionTilemap;
    [SerializeField] private Tilemap generatedDangerTilemap;

    private bool isPlaying;
    private float score;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement.canMove = false;
        scoreUI.enabled = false;
        mainMenuUI.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        // Enable game on first teleport
        if (!isPlaying && playerMovement.isTeleporting)
        {
            isPlaying = true;
            mainMenuUI.SetActive(false);
            scoreUI.enabled = true;
            playerMovement.canMove = true;
            spikeWall.canMove = true;
        }
    }

    private void GameOver()
    {
        // Stop playing
        isPlaying = false;
        spikeWall.canMove = false;

        // UI Stuff
        scoreUI.enabled = false;

        gameOverNewHighScoreUI.enabled = false;
        // Update highscore
        if (PlayerPrefs.GetInt("highscore", 0) < (int)score)
        {
            PlayerPrefs.SetInt("highscore", (int)score);
            gameOverNewHighScoreUI.enabled = true;
        }

        // Set text
        gameOverScoreUI.text = score + "";
        gameOverHighScoreUI.text = PlayerPrefs.GetInt("highscore", 0) + "";

        gameOverUI.SetActive(true);
    }

    public void ResetGame()
    {
        isPlaying = false;

        // Reset UI
        scoreUI.enabled = false;
        gameOverUI.SetActive(false);
        mainMenuUI.SetActive(true);
        playerMovement.canMove = false;

        //Reset level
        spikeWall.reset();
        levelGeneration.resetGeneration();
        generatedCollisionTilemap.ClearAllTiles();
        generatedDangerTilemap.ClearAllTiles();

        // Reset player and camera
        player.SetActive(true);
        player.transform.position = spawnPos;
        cam.transform.position = new Vector3(0,0, -10);
    }

    public void PlayerDied(int score)
    {
        // Gets called on players death

        this.score = score;
        GameOver();
    }


}
