using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "Input/Input Reader")]
public class InputReader : ScriptableObject, InputActions.IPlayerActions
{
    private InputActions inputActions;

    // Public properties
    public bool Tap { get; private set; }
    public Vector2 TouchPosition { get; private set; }
    public Vector2 StartDrag { get; private set; }

    // Events
    public event Action<Vector2> OnTapEvent;
    public event Action<Vector2> OnStartDragEvent;
    public event Action<Vector2> OnEndDragEvent;

    private void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new InputActions();
            inputActions.Player.SetCallbacks(this);
        }
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void OnDestroy()
    {
        inputActions.Dispose();
    }

    // Called when Tap button is pressed/released
    public void OnTap(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                Tap = true;
                OnTapEvent?.Invoke(TouchPosition); // use latest position
                break;
            case InputActionPhase.Canceled:
                Tap = false;
                break;
        }
    }

    // Called continuously while pointer moves
    public void OnPointerPosition(InputAction.CallbackContext context)
    {
        TouchPosition = context.ReadValue<Vector2>();
    }

    // Called when drag begins (button down)
    public void OnStartDrag(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                StartDrag = TouchPosition; // use pointer position
                OnStartDragEvent?.Invoke(StartDrag);
                Debug.Log("Start Drag at: " + StartDrag);
                break;
        }
    }

    // Called when drag ends (button up)
    public void OnEndDrag(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Canceled)
        {
            Vector2 endDrag = TouchPosition; // use pointer position
            OnEndDragEvent?.Invoke(endDrag);
            StartDrag = Vector2.zero;
            Debug.Log("End Drag at: " + endDrag);
        }
    }
}
