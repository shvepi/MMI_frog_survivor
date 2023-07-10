using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

    public static MenuManager instance;
    public bool playButtonPressed = false;

    public bool gameOver = false;

    [SerializeField]
    private Text scoreText, coinText, goScoreText, goCoinText;
    [SerializeField]
    private GameObject gameMenu, menu, gameOverMenu;

    private int currentCoin, currentScore;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Use this for initialization
    void Start ()
    {
        scoreText.text = "Score: " + 0;
        coinText.text = "Coins: " + 0;
	}

    public void IncreaseCoin()
    {
        currentCoin++;
        coinText.text = "Coins: " + currentCoin;
    }

    public void IncreaseScore()
    {
        currentScore++;
        scoreText.text = "Score: " + currentScore;
    }

    public void PlayBtn()
    {
        SoundManager.instance.PlayClick();
        menu.SetActive(false);
        gameMenu.SetActive(true);
        playButtonPressed = true; // Set the flag to true
    }

    public void GameOver()
    {
        SoundManager.instance.PlayDeath();
        gameOver = true;
        gameMenu.SetActive(false);
        gameOverMenu.SetActive(true);
        goScoreText.text = "Score: " + currentScore;
        goCoinText.text = "Coins: " + currentCoin;
    }

    public void HomeBtn()
    {
        SoundManager.instance.PlayClick();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
