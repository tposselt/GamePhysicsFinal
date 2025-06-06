using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// EventSO - A ScriptableObject-based event system that allows for decoupled communication between game components
/// This implements the observer pattern using UnityAction delegates
/// </summary>
[CreateAssetMenu(fileName = "Event", menuName = "Scriptable Objects/Events/Event Channel")]
public class EventChannelSO : ScriptableObjectBase
{
    // The collection of subscribers (listeners) to this event
    UnityAction listeners = null;

    /// <summary>
    /// Raises the event, notifying all subscribed listeners
    /// The ?. operator ensures we only invoke if there are active listeners
    /// </summary>
    public void Raise() => listeners?.Invoke();

    /// <summary>
    /// Adds a new listener to this event
    /// Call this method in OnEnable for any MonoBehaviour that needs to react to this event
    /// </summary>
    /// <param name="listener">The method to call when this event is raised</param>
    public void AddListener(UnityAction listener) => listeners += listener;

    /// <summary>
    /// Removes a listener from this event
    /// Call this method in OnDisable for any MonoBehaviour to prevent memory leaks
    /// </summary>
    /// <param name="listener">The method to remove from the invocation list</param>
    public void RemoveListener(UnityAction listener) => listeners -= listener;
}