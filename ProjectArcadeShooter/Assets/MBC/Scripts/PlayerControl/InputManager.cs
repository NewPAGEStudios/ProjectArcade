using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static InputManager _instance;
    public static InputManager Instance
    {
        get { return _instance; }
    }

    public bool crouching;


    private PlayerActionMaps inputActions;
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        inputActions = new PlayerActionMaps();

//        inputActions.playerMap.Crouch.performed += CrouchPerformed;
//        inputActions.playerMap.Crouch.canceled += CrouchCanceled;
    }

/*    private void CrouchPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        crouching = true;
    }
    private void CrouchCanceled(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Debug.Log("deneme");
        crouching = false;
    }
    public bool getCrouch()
    {
        return crouching;
    }
*/

    private void OnEnable()
    {
        inputActions.Enable();
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }

    public Vector2 getPlayerMovement()
    {
        return inputActions.playerMap.Movement.ReadValue<Vector2>();
    }
    public Vector2 getCameraMovement()
    {
        return inputActions.playerMap.Look.ReadValue<Vector2>();
    }

    public bool getJumpedPressed()
    {
        return inputActions.playerMap.Jump.triggered;
    }

    public bool getSlidePressed()
    {
        return inputActions.playerMap.Slide.triggered;
    }
    public bool getDashPressed()
    {
        return inputActions.playerMap.Dash.triggered;
    }
}