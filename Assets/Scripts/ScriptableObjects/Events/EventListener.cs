using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// EventListener - Component that listens for events from a ScriptableObject event channel
/// and invokes a UnityEvent response when the event is triggered.
/// </summary>
public class EventListener : MonoBehaviour
{
    [Tooltip("The ScriptableObject event channel to listen to")]
    [SerializeField] EventChannelSO eventChannel;

    [Tooltip("The UnityEvent response to trigger when the event is raised")]
    [SerializeField] UnityEvent response;

    /// <summary>
    /// When this component is enabled, subscribe to the event channel
    /// This follows the recommended pattern of subscribing in OnEnable
    /// </summary>
    private void OnEnable()
    {
        // Subscribe to the event only if the channel exists
        eventChannel?.AddListener(OnEventRaised);
    }

    /// <summary>
    /// When this component is disabled, unsubscribe from the event channel
    /// This prevents memory leaks and unintended behavior when the GameObject is inactive
    /// </summary>
    private void OnDisable()
    {
        // Unsubscribe from the event only if the channel exists
        eventChannel?.RemoveListener(OnEventRaised);
    }

    /// <summary>
    /// Called when the event is raised, invokes the configured UnityEvent response
    /// This allows for configuring event responses in the inspector without writing code
    /// </summary>
    private void OnEventRaised() => response?.Invoke();
}