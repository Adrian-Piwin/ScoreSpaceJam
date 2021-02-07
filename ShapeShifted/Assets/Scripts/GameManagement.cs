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

    [Header("UI")]
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject settingsUI;
    [SerializeField] private GameObject helpUI;
    [SerializeField] private GameObject storeUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private TextMeshProUGUI gameOverScoreUI;
    [SerializeField] private TextMeshProUGUI gameOverHighScoreUI;
    [SerializeField] private TextMeshProUGUI gameOverNewHighScoreUI;

    [Header("Level")]
    [SerializeField] private LevelGeneration levelGeneration;
    [SerializeField] private SpikeWallScript spikeWall;

    private bool isPlaying;
    private int score;
    public int coins;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement.canMove = false;
        gameplayUI.SetActive(false);
        mainMenuUI.SetActive(true);

        coins = PlayerPrefs.GetInt("Coin Amount", 0);
    }

    // Update is called once per frame
    void Update()
    {
        // Do not start game when in UI
        if (settingsUI.active || helpUI.active || storeUI.active)
            playerMovement.canTeleport = false;
        else
            playerMovement.canTeleport = true;

        // Enable game on first teleport
        if (!isPlaying && playerMovement.isTeleporting)
        {
            isPlaying = true;
            mainMenuUI.SetActive(false);
            gameplayUI.SetActive(true);
            playerMovement.canMove = true;
            spikeWall.canMove = true;
        }
    }

    private void GameOver()
    {
        // Stop playing
        isPlaying = false;
        spikeWall.canMove = false;

        // Update UI in gameover
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

        // Update coins
        PlayerPrefs.SetInt("Coin Amount", coins);

        gameOverUI.SetActive(true);
    }

    public void ResetGame()
    {
        isPlaying = false;

        // Reset UI
        gameplayUI.SetActive(false);
        gameOverUI.SetActive(false);
        mainMenuUI.SetActive(true);
        playerMovement.canMove = false;

        //Reset level
        spikeWall.reset();
        levelGeneration.resetGeneration();

        // Reset player and camera
        player.SetActive(true);
        player.transform.position = spawnPos;
        cam.transform.position = new Vector3(0,0, -10);
    }

    public void PlayerDied(int score, int coins)
    {
        // Gets called on players death

        this.score = score;
        this.coins += coins;
        GameOver();
    }


}
