using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    private const int COIN_SCORE_AMOUNT = 5;
    public static GameManager Instance { set; get; }
    private bool isGameStarted = false;
    public bool IsDead { set; get; }
    private PlayerMotor motor;

    public Animator gameCanvas, menuAnim, penguideAnim;
    public TextMeshProUGUI scoreText, coinText, modifierText, hiScoreText;
    private float score, coinScore, modifierScore;
    private int lastScore;

    public Animator deathMenuAnim;
    public TextMeshProUGUI deadScoreText, deadCoinText;

    private void Awake()
    {
        Instance = this;
        modifierScore = 1;
        modifierText.text = "x" + modifierScore.ToString("0.0");
        scoreText.text = score.ToString("0");
        coinText.text = coinScore.ToString("01;");
        motor = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>();

        hiScoreText.text = PlayerPrefs.GetInt("HiScore").ToString();
    }

    public void Update()
    {
        if(MobileInput.Instance.Tap && !isGameStarted)
        {
            isGameStarted = true;
            //Debug.Log("I felt a touch");
            motor.StartRunning();
            FindObjectOfType<GlacierSpawner>().IsScrolling = true;

            FindObjectOfType<CameraMotor>().IsMoving = true;
            gameCanvas.SetTrigger("Show");
            menuAnim.SetTrigger("Hide");
        }

        if (isGameStarted && !IsDead)
        {
            score += (Time.deltaTime * modifierScore);
            if(lastScore != (int)score)
            {
                lastScore = (int)score;
                scoreText.text = score.ToString("0");
            }
            
        }
    }

    public void GetCoin()
    {
        coinScore++;
        coinText.text = coinScore.ToString("0");
        score += COIN_SCORE_AMOUNT;
        scoreText.text = score.ToString("0");
        penguideAnim.SetTrigger("Collect");
    }

    public void UpdateModifier(float modifierAmount)
    {
        modifierScore = 1.0f + modifierAmount;
        modifierText.text = "x" + modifierScore.ToString("0.0");
    }

    public void OnPlayButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game"); 
    }

    public void OnDeath()
    {
        IsDead = true;
        deadScoreText.text = score.ToString("0");
        deadCoinText.text = coinScore.ToString("0");
        deathMenuAnim.SetTrigger("Dead");
        FindObjectOfType<GlacierSpawner>().IsScrolling = false;
        gameCanvas.SetTrigger("Hide");

        //Check if this is a highscore.
        if (score > PlayerPrefs.GetInt("HiScore"))
        {
            float    s = score;
            if (s % 1 == 0)
                s += 1;
            PlayerPrefs.SetInt("HiScore", (int)s);
        }
    }
}
