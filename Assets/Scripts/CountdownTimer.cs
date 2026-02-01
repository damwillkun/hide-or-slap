using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    public TextMeshProUGUI TimerText;
    public GameObject CurrentRound;
    public TextMeshProUGUI CurrentRoundText;

    private Image backgroundImage;
    private float currentTime;
    private bool isRunning = false;

    private void Awake()
    {
        backgroundImage = GetComponent<Image>();
        TimerText.enabled = false;
        CurrentRoundText.text = "ROUND 1";
        CurrentRound.SetActive(false);
    }

    void Update()
    {
        if (!isRunning) return;

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            currentTime = Mathf.Max(currentTime, 0);
            UpdateTimerDisplay();
        }
        else
        {
            backgroundImage.enabled = false;
            TimerText.enabled = false;
            CurrentRound.SetActive(false);
            isRunning = false;
            // Ici tu peux déclencher un événement quand le timer est fini
            // Exemple : Debug.Log("Timer terminé !");
        }
    }

    public void StartTimer(int duration)
    {
        currentTime = duration;
        UpdateTimerDisplay();

        backgroundImage.enabled = true;
        TimerText.enabled = true;
        CurrentRound.SetActive(true);
        isRunning = true;
    }

    public void StopTimer()
    {
        backgroundImage.enabled = false;
        TimerText.enabled = false;
        CurrentRound.SetActive(false);
        isRunning = false;
    }

    void UpdateTimerDisplay()
    {
        //int seconds = Mathf.FloorToInt(currentTime);
        //int milliseconds = Mathf.FloorToInt((currentTime - seconds) * 1000);

        //TimerText.text = $"{seconds:00}:{milliseconds:00}";
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        int milliseconds = Mathf.FloorToInt((currentTime * 1000f) % 1000f);

        TimerText.text = $"{seconds:00}:{milliseconds:000}";
    }

    public void UpdateRound()
    {
        CurrentRoundText.text = "ROUND " + GameManager.Instance.CurrentRound;
    }
}
