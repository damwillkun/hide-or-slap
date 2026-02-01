using TMPro;
using UnityEngine;
using System.Collections;

public class ScoreAnim : MonoBehaviour
{
    [Header("Settings")]
    public float lifetime = 1f;
    public float moveUpDistance = 50f;
    public float scaleMultiplier = 1.2f;
    public Color goodScoreColor = Color.white;
    public Color badScoreColor = Color.white;

    private TextMeshProUGUI text;
    private Vector3 startPos;
    private Color startColor;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        startPos = transform.localPosition;

        text.enabled = false;
    }

    public void PlayScore(int scoreToDisplay)
    {
        string sign = scoreToDisplay > 0 ? "+" : "-";
        text.color = scoreToDisplay > 0 ? goodScoreColor : badScoreColor;
        string toDisplay = sign + Mathf.Abs(scoreToDisplay);

        text.text = toDisplay;
        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        float t = 0f;
        text.enabled = true;
        startColor = text.color;

        while (t < lifetime)
        {
            float normalized = t / lifetime;

            transform.localPosition = startPos + Vector3.up * moveUpDistance * normalized;

            float scale = Mathf.Lerp(1f, scaleMultiplier, normalized);
            transform.localScale = Vector3.one * scale;

            text.color = new Color(
                startColor.r,
                startColor.g,
                startColor.b,
                1f - normalized
            );

            t += Time.deltaTime;
            yield return null;
        }

        text.enabled = false;
    }
}
