using System.Collections.Generic;


/// <summary>
/// Base interface for all events
/// </summary>
public interface IEvent { }


/// <summary>
/// Static generic event bus - each event type gets its own bus
/// Provides type-safe, high-performance event routing
/// </summary>
public static class EventBus<T> where T : IEvent
{
    private static readonly HashSet<IEventBinding<T>> bindings = new HashSet<IEventBinding<T>>();


    /// <summary>
    /// Subscribe an EventBinding to this event type
    /// </summary>
    public static void Subscribe(EventBinding<T> binding) => bindings.Add(binding);


    /// <summary>
    /// Unsubscribe an EventBinding from this event type
    /// </summary>
    public static void Unsubscribe(EventBinding<T> binding) => bindings.Remove(binding);


    /// <summary>
    /// Raise an event, notifying all subscribers
    /// </summary>
    public static void Publish(T @event)
    {
        foreach (var binding in bindings)
        {
            binding.OnEvent?.Invoke(@event);
            binding.OnEventNoArgs?.Invoke();
        }
    }
     

    /// <summary>
    /// Clear all subscriptions (useful for scene transitions)
    /// </summary>
    public static void Clear() => bindings.Clear();
}