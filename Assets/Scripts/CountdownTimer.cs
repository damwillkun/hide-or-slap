using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    private TextMeshProUGUI timerText;
    private float currentTime;
    private bool isRunning = false;

    private void Awake()
    {
        timerText = GetComponent<TextMeshProUGUI>();
        timerText.enabled = false;
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
            timerText.enabled = false;
            isRunning = false;
            // Ici tu peux déclencher un événement quand le timer est fini
            // Exemple : Debug.Log("Timer terminé !");
        }
    }

    public void StartTimer(int duration)
    {
        currentTime = duration;
        UpdateTimerDisplay();

        timerText.enabled = true;
        isRunning = true;
    }

    public void StopTimer()
    {
        timerText.enabled = false;
        isRunning = false;
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}
