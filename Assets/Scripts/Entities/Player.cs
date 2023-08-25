using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour, Controls.IPlayerActions
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
        _Controls.Player.SetCallbacks(this);
    }



    private void OnDisable()
    {
        _Controls.Disable();
        _Controls.Player.SetCallbacks(null);
    }



    public void OnMovement(InputAction.CallbackContext inputActionCallbackContext)
    {
        if (inputActionCallbackContext.started == true)
        {
            _IsMoveKeyHeld = true;
        }
        else if (inputActionCallbackContext.canceled == true)
        {
            _IsMoveKeyHeld = false;
        }
    }



    public void OnExit(InputAction.CallbackContext inputActionCallbackContext)
    {
        if (inputActionCallbackContext.performed == true)
        {
            Action.EscapeAction();
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
        //Debug.Log("MovePlayer");

        Controls.PlayerActions playerActions = _Controls.Player;
        InputAction inputAction = playerActions.Movement;
        
        Vector2 movementVector = inputAction.ReadValue<Vector2>();

        // Round each movement delta to the nearest integer.  This helps with diagonal movement.
        Vector2 roundedMovementVector = new Vector2(Mathf.Round(movementVector.x), Mathf.Round(movementVector.y));

        //Debug.Log($"roundedMovementVector: {roundedMovementVector}");

        Vector3 newPosition = transform.position + (Vector3) roundedMovementVector;
        if (IsValidPosition(newPosition) == false)
        {
            //Debug.Log("Movement denied.");
            return;
        }

        Action.MovementAction(GetComponent<Entity>(), roundedMovementVector);
    }



    private bool IsValidPosition(Vector3 position)
    {
        if (position == transform.position)
        {
            // We are already there
            return false;
        }

        MapManager mapManager = MapManager.Instance;

        Tilemap floorMap = mapManager.FloorMap;        
        Vector3Int gridPosition = floorMap.WorldToCell(position);

        if (mapManager.InBounds(gridPosition.x, gridPosition.y) == false)
        {
            // Out of map bounds
            return false;
        }

        Tilemap obstacleMap = mapManager.ObstacleMap;
        if(obstacleMap.HasTile(gridPosition) == true)
        {
            // An obstacle stands in the way
            return false;
        }

        return true;
    }
}