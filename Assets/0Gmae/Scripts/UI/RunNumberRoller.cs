using UnityEngine;
using TMPro;
using DG.Tweening;

public class RunNumberRoller : MonoBehaviour
{
    public bool activeOnEnable = true;

    [Header("Setting")]
    public float defaultTarget = 20f;
    public float defaultDuration = 2f;
    public float maxJitter = 50f;
    public float overshootPercent = 0.3f;
    public int decimalPlaces = 0;

    [Header("Text Setting")]
    public string prefix = "";
    public string suffix = "";

    float currentValue = 0f;
    TMP_Text tmp;
    Tween tween;

    private void Awake()
    {
        tmp = GetComponent<TMP_Text>();
    }

    void OnEnable()
    {
        //ResetDisplay();
        if (activeOnEnable) StartNumber();
    }

    void OnDisable()
    {
        //ResetDisplay();
    }

    public void ResetDisplay()
    {
        tween?.Kill();
        currentValue = 0f;
        UpdateUI();
    }

    public void StartNumberWithTarget(float target)
    {
        RunTween(target, defaultDuration);
    }

    public void StartNumberWithDuration(float duration)
    {
        RunTween(defaultTarget, duration);
    }

    public void StartNumber(float target, float duration)
    {
        RunTween(target, duration);
    }

    public void StartNumber()
    {
        RunTween(defaultTarget, defaultDuration);
    }

    void RunTween(float target, float duration)
    {
        tween?.Kill();
        currentValue = 0f;
        UpdateUI();

        float pseudoTarget = Mathf.Abs(target) < 0.001f ? maxJitter : target;

        tween = DOVirtual.Float(0f, pseudoTarget, duration, (value) =>
        {
            float t = tween.ElapsedPercentage();
            float jitterStrength = Mathf.Lerp(maxJitter, 0f, t);
            float overshoot = Mathf.Abs(target) * overshootPercent;
            float jitter = Random.Range(-(jitterStrength + overshoot), jitterStrength + overshoot);

            currentValue = value + jitter;
            UpdateUI();
        })
        .SetEase(Ease.OutQuad)
        .OnComplete(() =>
        {
            currentValue = target;
            UpdateUI();
        });
    }

    void UpdateUI()
    {
        if (tmp == null) return;
        if (decimalPlaces <= 0)
            tmp.text = prefix + Mathf.RoundToInt(currentValue).ToString() + suffix;
        else
            tmp.text = prefix + currentValue.ToString("F" + decimalPlaces) + suffix;
    }
}
