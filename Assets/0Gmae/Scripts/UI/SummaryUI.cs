using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SummaryUI : MonoBehaviour
{
    public TMP_Text ratingText;
    public TMP_Text scoreText;
    public Image[] starFills;
    public StateUI[] stateUIs;

    private List<bool> resultList;

    public void SetupUI(List<bool> _resultList)
    {
        resultList = _resultList ?? new List<bool>();

        if (stateUIs == null || stateUIs.Length == 0) return;

        if (stateUIs.Length != resultList.Count)
            Debug.LogWarning($"SummaryUI: stateUIs({stateUIs.Length}) != resultList({resultList.Count})");

        float percent = GetPercentScore();
        UpdateRatingUI(percent);
        UpdateStars(percent);
        UpdateStateIcons();
    }

    void UpdateRatingUI(float percent)
    {
        if (ratingText) ratingText.text = (percent * 5f).ToString("0.0");
        if (scoreText) scoreText.text = $"You have earned {TotalScore()} of {stateUIs.Length} points";
    }

    void UpdateStars(float percent)
    {
        if (starFills == null || starFills.Length == 0) return;

        float starValue = percent * starFills.Length;

        for (int i = 0; i < starFills.Length; i++)
        {
            float fill = Mathf.Clamp01(starValue - i);
            starFills[i].fillAmount = fill;
        }
    }

    void UpdateStateIcons()
    {
        int count = Mathf.Min(stateUIs.Length, resultList.Count);

        for (int i = 0; i < count; i++)
            stateUIs[i].SetIcon(resultList[i]);

        for (int i = count; i < stateUIs.Length; i++)
            stateUIs[i].SetIcon(false);
    }

    float GetPercentScore()
    {
        if (stateUIs == null || stateUIs.Length == 0) return 0f;
        return TotalScore() / (float)stateUIs.Length;
    }

    int TotalScore()
    {
        int total = 0;
        int count = Mathf.Min(stateUIs.Length, resultList.Count);

        for (int i = 0; i < count; i++)
            if (resultList[i]) total++;

        return total;
    }
}
