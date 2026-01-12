using UnityEngine;

public class InputBusReader : MonoBehaviour
{
    [Space(20)]
    [Header("Input Bus Stream")]
    [SerializeField] private InputBus inputBus;

    private Vector2 moveInput;
    private bool isJumping;



    private void Start()
    {
        inputBus.MoveEvent += SetMove;
        inputBus.JumpEvent += SetJump;
        inputBus.PauseEvent += PauseGame;
        inputBus.EnableInputAction();
    }

    private void Update()
    {
        HandleMove();
        HandleJump();
    }



    #region Get Input Value
    private void SetMove(Vector2 moveInput) 
    {
        this.moveInput = moveInput;
    }
    private void SetJump(bool isJumping)
    {
        this.isJumping = isJumping;
    }
    #endregion



    #region Handle Input Value
    private void HandleMove() 
    {
        Debug.Log($"Move Input: {moveInput}");
    }
    private void HandleJump()
    {
        if (isJumping) Debug.Log("Jump!");
    }
    #endregion


    private void PauseGame() 
    {
        Debug.Log("Pause Game");
    }
}
