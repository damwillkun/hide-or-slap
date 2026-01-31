using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI TextTitleRound;
    public GameObject SequenceTitlePrepareAction;
    public GameObject SequenceTitlePlayAction;
    public CountdownTimer PrepareActionCountdownTimer;

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
}
