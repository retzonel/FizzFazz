using UnityEngine;

public class InputTester : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;

    private void OnEnable()
    {
        if (inputReader != null)
        {
            inputReader.OnTapEvent += HandleTap;
            inputReader.OnStartDragEvent += HandleStartDrag;
            inputReader.OnEndDragEvent += HandleEndDrag;
        }
    }

    private void OnDisable()
    {
        if (inputReader != null)
        {
            inputReader.OnTapEvent -= HandleTap;
            inputReader.OnStartDragEvent -= HandleStartDrag;
            inputReader.OnEndDragEvent -= HandleEndDrag;
        }
    }

    private void HandleTap(Vector2 position)
    {
        Debug.Log($"Tap detected");
    }

    private void HandleStartDrag(Vector2 startPosition)
    {
        Debug.Log($"Drag started at position: {startPosition}");
    }

    private void HandleEndDrag(Vector2 endPosition)
    {
        Debug.Log($"Drag ended at position: {endPosition}");
    }
}