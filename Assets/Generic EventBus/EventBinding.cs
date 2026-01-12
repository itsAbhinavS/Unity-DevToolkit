using System;


/// <summary>
/// Interface for event bindings that can hold callbacks with or without event data
/// </summary>
internal interface IEventBinding<T> where T : IEvent
{
    Action<T> OnEvent { get; set; }
    Action OnEventNoArgs { get; set; }
}


/// <summary>
/// Wraps event callbacks and manages subscription to EventBus
/// Supports callbacks both with and without event data
/// </summary>
public class EventBinding<T> : IEventBinding<T> where T : IEvent
{
    private Action<T> onEvent = _ => { };
    private Action onEventNoArgs = () => { };

    // Explicit interface implementation - forces use of Add/Remove methods
    Action<T> IEventBinding<T>.OnEvent
    {
        get => onEvent;
        set => onEvent = value;
    }
    Action IEventBinding<T>.OnEventNoArgs
    {
        get => onEventNoArgs;
        set => onEventNoArgs = value;
    }

    // Constructors
    public EventBinding(Action<T> onEvent) => this.onEvent = onEvent;
    public EventBinding(Action onEventNoArgs) => this.onEventNoArgs = onEventNoArgs;

    // Add/Remove methods for managing callbacks
    public void Add(Action<T> onEvent) => this.onEvent += onEvent;
    public void Remove(Action<T> onEvent) => this.onEvent -= onEvent;
    public void Add(Action onEventNoArgs) => this.onEventNoArgs += onEventNoArgs;
    public void Remove(Action onEventNoArgs) => this.onEventNoArgs -= onEventNoArgs;
}