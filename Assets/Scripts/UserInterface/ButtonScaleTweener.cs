using UnityEngine;
using DG.Tweening; // DOTween namespace

public class ButtonScaleTweener : MonoBehaviour
{
    private RectTransform targetButton;
    [SerializeField] private float minScale = 0.8f; // Minimum scale
    [SerializeField] private float maxScale = 1.2f; // Maximum scale
    [SerializeField] private float tweenDuration = 0.5f; // Time to scale up or down

    private Vector3 initialScale;

    void Start()
    {
        targetButton = GetComponent<RectTransform>();

        if (targetButton == null)
        {
            Debug.LogError("Target Button is not assigned!");
            return;
        }

        initialScale = targetButton.localScale;

        // Animate scale back and forth using DOTween
        targetButton.DOScale(initialScale * maxScale, tweenDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
}