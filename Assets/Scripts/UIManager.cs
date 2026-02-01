using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI TextTitleRound;
    public GameObject SequenceTitlePrepareAction;
    public GameObject SequenceTitlePlayAction;
    public GameObject VersusSeparator;
    public CountdownTimer PrepareActionCountdownTimer;
    [Header("Player 1")]
    public TextMeshProUGUI ScoringValueP1;
    public TextMeshProUGUI UpdateScoreValueP1;
    [Header("Player 2")]
    public TextMeshProUGUI ScoringValueP2;
    public TextMeshProUGUI UpdateScoreValueP2;

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

        PrepareActionCountdownTimer.gameObject.SetActive(false);

        ScoringValueP1.gameObject.SetActive(false);
        ScoringValueP1.text= "0";
        UpdateScoreValueP1.gameObject.SetActive(false);
        ScoringValueP1.gameObject.SetActive(false);
        ScoringValueP2.text = "0";
        UpdateScoreValueP2.gameObject.SetActive(false);

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

    public void DisplayVersus(bool displayed)
    {
        VersusSeparator.gameObject.SetActive(displayed);
        ScoringValueP1.gameObject.SetActive(true);
        ScoringValueP1.gameObject.SetActive(true);
    }
}
