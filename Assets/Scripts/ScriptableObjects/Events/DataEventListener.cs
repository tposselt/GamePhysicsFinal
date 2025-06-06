using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Generic event listener for events that pass data of type T.
/// Connects ScriptableObject-based data events to UnityEvent responses with matching type.
/// </summary>
/// <typeparam name="T">The type of data this event will handle</typeparam>
public class DataEventListener<T> : MonoBehaviour
{
    [Tooltip("The data event channel to listen to")]
    [SerializeField] DataEventChannelSO<T> eventChannel;

    [Tooltip("The response to invoke when the event is raised with data")]
    [SerializeField] UnityEvent<T> response;

    /// <summary>
    /// When this component is enabled, subscribe to the data event channel.
    /// Uses null conditional operator to safely handle unassigned references.
    /// </summary>
    private void OnEnable()
    {
        eventChannel?.AddListener(OnEventRaised);
    }

    /// <summary>
    /// When this component is disabled, unsubscribe from the data event channel.
    /// Prevents memory leaks and ensures proper cleanup.
    /// </summary>
    private void OnDisable()
    {
        eventChannel?.RemoveListener(OnEventRaised);
    }

    /// <summary>
    /// Called when the event is raised, passes the data parameter to the response.
    /// Acts as a bridge between the ScriptableObject event system and Unity's inspector-configurable events.
    /// </summary>
    /// <param name="parameter">The data received from the event</param>
    private void OnEventRaised(T parameter)
    {
        response?.Invoke(parameter);
    }
}