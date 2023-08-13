using UnityEngine;
using UnityEngine.InputSystem;



public class Player : MonoBehaviour
{
    private Controls _Controls;

    [SerializeField] private bool _IsMoveKeyHeld = false;



    private void Awake()
    {
        _Controls = new Controls();
    }



    private void OnEnable()
    {
        _Controls.Enable();

        _Controls.Player.Movement.started += OnMovement;
        _Controls.Player.Movement.canceled += OnMovement;
        _Controls.Player.Exit.performed += OnExit;
    }



    private void OnDisable()
    {
        _Controls.Disable();

        _Controls.Player.Movement.started -= OnMovement;
        _Controls.Player.Movement.canceled -= OnMovement;
        _Controls.Player.Exit.performed -= OnExit;
    }



    private void OnExit(UnityEngine.InputSystem.InputAction.CallbackContext inputActionCallbackContext)
    {
        Debug.Log("Exit");
        Application.Quit();
    }



    private void OnMovement(UnityEngine.InputSystem.InputAction.CallbackContext inputActionCallbackContext)
    {
        if(inputActionCallbackContext.started == true)
        {
            _IsMoveKeyHeld = true;
        }
        else if(inputActionCallbackContext.canceled == true)
        {
            _IsMoveKeyHeld = false;
        }
    }



    private void FixedUpdate()
    {
        GameManager gameManager = GameManager.Instance;
        if(gameManager.IsPlayerTurn == true && _IsMoveKeyHeld == true)
        {
            MovePlayer();
        }
    }



    private void MovePlayer()
    {
        Debug.Log("MovePlayer");

        Controls.PlayerActions playerActions = _Controls.Player;
        InputAction inputAction = playerActions.Movement;
        Vector2 movementVector = inputAction.ReadValue<Vector2>();
        
        transform.position += (Vector3) movementVector;

        GameManager gameManager = GameManager.Instance;
        gameManager.EndTurn();
    }
}