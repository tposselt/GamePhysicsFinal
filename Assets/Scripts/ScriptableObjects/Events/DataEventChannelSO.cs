using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A generic ScriptableObject-based event system that can pass typed data to listeners.
/// Implements the observer pattern using UnityAction delegates with a generic parameter.
/// </summary>
public class DataEventChannelSO<T> : ScriptableObjectBase
{
    /// <summary>
    /// The collection of listeners subscribed to this event.
    /// Each listener accepts a parameter of type T when invoked.
    /// </summary>
    private UnityAction<T> listeners = null;

    /// <summary>
    /// Raises the event with the specified value, notifying all listeners.
    /// The ?. operator ensures we only invoke if there are active listeners.
    /// </summary>
    /// <param name="value">The data to pass to all listeners</param>
    public void Raise(T value) => listeners?.Invoke(value);

    /// <summary>
    /// Adds a new listener to this event.
    /// Call this method in OnEnable for any MonoBehaviour that needs to react to this event.
    /// </summary>
    /// <param name="listener">The method to call when this event is raised</param>
    public void AddListener(UnityAction<T> listener) => listeners += listener;

    /// <summary>
    /// Removes a listener from this event.
    /// Call this method in OnDisable for any MonoBehaviour to prevent memory leaks.
    /// </summary>
    /// <param name="listener">The method to remove from the invocation list</param>
    public void RemoveListener(UnityAction<T> listener) => listeners -= listener;
}