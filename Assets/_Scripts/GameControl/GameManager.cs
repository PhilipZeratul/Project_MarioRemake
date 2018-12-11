using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;


public class GameManager : MonoBehaviour
{
    public int score;
    public int coins;
    public int countDown;
    public int lives = 3;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI worldText;
    public TextMeshProUGUI countDownText;

    public List<SceneReference> sceneList = new List<SceneReference>();

    private readonly int defaultCountDown = 400;
    private readonly WaitForSeconds waitOneSecond = new WaitForSeconds(1);
    private Coroutine countDownCoroutine;


    private void Start()
    {
        SceneManager.LoadSceneAsync(sceneList[0].ScenePath, LoadSceneMode.Additive);              
        worldText.text = sceneList[0].SceneName;

        countDown = defaultCountDown;
        countDownCoroutine = StartCoroutine(CountDown());
    }

    private IEnumerator CountDown()
    {
        while (countDown > 0)
        {
            yield return waitOneSecond;
            AddCountDown(-1);
        }
    }

    public void AddScore(int add)
    {
        score += add;
        if (score > 999999)
            score = 999999;
        scoreText.text = score.ToString("000000");
    }

    public void AddCoin(int add)
    {
        coins += add;
        if (coins > 99)
        {
            AddLives(1);
            coins = 0;
        }
        coinText.text = coins.ToString("*00");
    }

    public void AddLives(int add)
    {
        lives += add;
    }

    public void AddCountDown(int add)
    {
        countDown += add;
        if (countDown < 0)
            GameOver();

        countDownText.text = countDown.ToString("000");
    }

    private void GameOver()
    {
        Debug.Log("Game Over!");
    }
}
