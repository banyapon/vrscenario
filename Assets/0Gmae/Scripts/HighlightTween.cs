using UnityEngine;
using DG.Tweening;

public class HighlightTween : MonoBehaviour
{
    [SerializeField] private Material targetMaterial;

    private string vignetteColorProperty = "_VignetteColor";
    private string vignetteBlendProperty = "_VignetteColorBlend";

    [SerializeField] private float duration = 0.5f;
    [SerializeField] private Ease ease = Ease.InOutSine;

    [Range(0f, 1f)] [SerializeField] private float maxAlpha = 0.25f;
    [Range(0f, 1f)] [SerializeField] private float minAlpha = 0f;

    private Tween alphaTween;
    private bool forward = true;

    private void OnEnable()
    {
        StartLoop();
    }

    private void OnDisable()
    {
        StopLoop();
    }

    public void StartLoop()
    {
        if (targetMaterial == null) return;

        StopLoop();

        float from = forward ? minAlpha : maxAlpha;
        float to = forward ? maxAlpha : minAlpha;

        alphaTween = DOTween.To(
            () => from,
            value => SetVignetteAlpha(value),
            to,
            duration
        )
        .SetEase(ease)
        .OnComplete(() =>
        {
            forward = !forward;
            StartLoop();
        });
    }

    public void StopLoop()
    {
        if (alphaTween != null)
        {
            alphaTween.Kill();
            alphaTween = null;
        }
    }

    private void SetVignetteAlpha(float alpha)
    {
        if (targetMaterial.HasProperty(vignetteColorProperty))
        {
            Color c = targetMaterial.GetColor(vignetteColorProperty);
            c.a = alpha;
            targetMaterial.SetColor(vignetteColorProperty, c);
        }

        if (targetMaterial.HasProperty(vignetteBlendProperty))
        {
            Color c = targetMaterial.GetColor(vignetteBlendProperty);
            c.a = alpha;
            targetMaterial.SetColor(vignetteBlendProperty, c);
        }
    }

}
