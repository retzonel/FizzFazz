using UnityEngine;

namespace Creotly.FizzFazz
{
    public class InputTester : MonoBehaviour
    {
        public bool debugMode;
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
            if(debugMode) Debug.Log($"Tap detected");
        }

        private void HandleStartDrag(Vector2 startPosition)
        {
            if (debugMode) Debug.Log($"Drag started at position: {startPosition}");
        }

        private void HandleEndDrag(Vector2 endPosition)
        {
            if (debugMode) Debug.Log($"Drag ended at position: {endPosition}");
        }
    }
}
