using UnityEngine;
using DG.Tweening; // import DOTween namespace

public class UIRotateBackAndForthTween : MonoBehaviour
{
    private RectTransform targetUIElement;
    [SerializeField] private float rotateZAngle = 15f; // Maximum rotation angle
    [SerializeField] public float tweenDuration = 1f; // Duration for one side of rotation

    private Vector3 initialRotation;

    void Start()
    {
        targetUIElement = GetComponent<RectTransform>();

        if (targetUIElement == null)
        {
            Debug.LogError("Target UI Element is not assigned!");
            return;
        }

        initialRotation = targetUIElement.localEulerAngles;

        // Create DOTween rotation tween
        targetUIElement
            .DOLocalRotate(initialRotation + new Vector3(0, 0, rotateZAngle), tweenDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo); // Infinite back & forth
    }
}