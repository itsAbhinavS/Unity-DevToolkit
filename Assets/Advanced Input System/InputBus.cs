using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static GameInputSystem;



/// <summary>
/// Interface for exposing player input functionality via an Input Bus pattern.
/// </summary>
public interface IInputBus 
{
    public void EnableInputAction();
}



/// <summary>
/// Centralized input bus for handling player and UI input events.
/// 
/// Usage:
/// <para>
/// 1. Create an instance via <see cref="CreateAssetMenu"/> ("Input/InputBus").
/// 2. Reference the asset in your systems (Player, UI, Managers).
/// 3. Subscribe to events like <see cref="MoveEvent"/>, <see cref="JumpEvent"/>.
/// 4. Call <see cref="EnableInputAction"/> at runtime to activate.
/// </para>
/// 
/// </summary>
[CreateAssetMenu(fileName = "InputBus")]
public class InputBus : ScriptableObject, IInputBus, IPlayerActions, IUIActions
{
    public event UnityAction<Vector2> MoveEvent = delegate { };
    public event UnityAction<bool> JumpEvent = delegate { };
    public event UnityAction PauseEvent = delegate { };

    public GameInputSystem inputActions;

    public Vector2 Move => inputActions.Player.Move.ReadValue<Vector2>();

    public void EnableInputAction() 
    {
        if (inputActions == null) 
        {
            inputActions = new GameInputSystem();
            inputActions.Player.SetCallbacks(this);
            inputActions.UI.SetCallbacks(this);
        }
        inputActions.Enable();
    }

    public void OnMove(InputAction.CallbackContext ctx) 
    {
        MoveEvent.Invoke(inputActions.Player.Move.ReadValue<Vector2>());
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        switch (ctx.phase) 
        {
            case InputActionPhase.Started:
                JumpEvent(true);
                break;
            case InputActionPhase.Canceled:
                JumpEvent(false);
                break;
        }
    }
    public void OnPause(InputAction.CallbackContext ctx)
    {
        PauseEvent.Invoke();
    }
}