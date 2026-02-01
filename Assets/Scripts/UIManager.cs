using System;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject MainMenu;
    public TextMeshProUGUI TextTitleRound;
    public GameObject SequenceTitlePrepareAction;
    public GameObject SequenceTitlePlayAction;
    public CountdownTimer PrepareActionCountdownTimer;
    public TextMeshProUGUI EndGame;
    [Space]
    public GameObject VersusDisplay;

    [Header("Scores")]
    public Scoring Scoring;
    public ScoreAnim ScoreAnimP1;
    public ScoreAnim ScoreAnimP2;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        HideTitles();

        EndGame.gameObject.SetActive(false);
        VersusDisplay.gameObject.SetActive(false);

        DisplayMainTitle(true);

        PrepareActionCountdownTimer.gameObject.SetActive(false);

        DisplayVersus(false);
    }

    public void HideTitles()
    {
        TextTitleRound.gameObject.SetActive(false);
        SequenceTitlePrepareAction.SetActive(false);
        SequenceTitlePlayAction.SetActive(false);
    }

    public void DisplayRound(int round)
    {
        HideTitles();

        TextTitleRound.text = "ROUND " + round;
        TextTitleRound.gameObject.SetActive(true);
    }

    public void DisplayMainTitle(bool displayed)
    {
        MainMenu.gameObject.SetActive(displayed);
    }

    public void DisplayVersus(bool displayed)
    {
        VersusDisplay.gameObject.SetActive(displayed);
        Scoring.gameObject.SetActive(displayed);
    }

    public void UpdateScore(int indexPlayer, int score)
    {
        if(indexPlayer == 1)
        {
            ScoreAnimP1.PlayScore(score);
        }
        else if(indexPlayer == 2)
        {
            ScoreAnimP2.PlayScore(score);
        }
    }

    public void DisplayWinner(int indexWinnerPlayer)
    {
        if(indexWinnerPlayer > 0)
        {
            EndGame.text = "PLAYER " + indexWinnerPlayer + " WINS!";
            EndGame.gameObject.SetActive(true);
        }
        else
        {
            EndGame.text = "EQUALITY";
        }
    }
}
