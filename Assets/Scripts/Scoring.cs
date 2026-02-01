using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class Scoring : MonoBehaviour
{
    public RectTransform ScoreBarP1;
    public RectTransform ScoreBarP2;

    private void Start()
    {
        ScoreBarP1.sizeDelta = Vector2.zero;
        ScoreBarP2.sizeDelta = Vector2.zero;
    }

    public void UpdateScore()
    {
        float scoreP1 = GameManager.Instance.Player1.CurrentScore;
        float scoreP2 = GameManager.Instance.Player2.CurrentScore;

        //P1
        float normalized = Mathf.Clamp01(scoreP1 / GameManager.Instance.MaxScore);
        float width = normalized * 960;

        var leftSize = ScoreBarP1.sizeDelta;
        leftSize.x = width;
        ScoreBarP1.sizeDelta = leftSize;

        //P2
        normalized = Mathf.Clamp01(scoreP2 / GameManager.Instance.MaxScore);
        width = normalized * 960;

        var rightSize = ScoreBarP2.sizeDelta;
        rightSize.x = width;
        ScoreBarP2.sizeDelta = rightSize;
    }
}
