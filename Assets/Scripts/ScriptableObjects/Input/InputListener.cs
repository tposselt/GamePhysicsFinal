using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// Bridges between input system events and Unity Events.
/// Listens to InputActionSO events and forwards them to UnityEvents
/// that can be connected in the Inspector.
/// </summary>
public class InputListener : MonoBehaviour
{
    [SerializeField] InputActionSO inputAction;

    [Header("Unity Events")]
    public UnityEvent<Vector2> OnVector2Input;
    public UnityEvent<float> OnFloatInput;
    public UnityEvent OnButtonPressed;
    public UnityEvent OnButtonReleased;

    public InputActionSO InputAction => inputAction;

    private void OnEnable()
    {
        // Skip initialization if no input action is assigned
        if (inputAction == null)
        {
            Debug.LogWarning("InputAction is not assigned in " + gameObject.name);
            return;
        }

        // Initialize the input action (sets up the underlying input system connections)
        inputAction.Initialize();

        // Register Unity Event handlers to corresponding input action events		
        inputAction.OnVector2Input += OnVector2Input.Invoke;
        inputAction.OnFloatInput += OnFloatInput.Invoke;
        inputAction.OnButtonPressed += OnButtonPressed.Invoke;
        inputAction.OnButtonReleased += OnButtonReleased.Invoke;
    }

    private void OnDisable()
    {
        // Skip cleanup if no input action is assigned
        if (inputAction == null) return;

        // Unregister Unity Event handlers from input action events
        // This prevents memory leaks and ensures proper cleanup
        inputAction.OnVector2Input -= OnVector2Input.Invoke;
        inputAction.OnFloatInput -= OnFloatInput.Invoke;
        inputAction.OnButtonPressed -= OnButtonPressed.Invoke;
        inputAction.OnButtonReleased -= OnButtonReleased.Invoke;

        // Clean up the input action (releases any resources held by the input system)
        inputAction.Deinitialize();
    }
}