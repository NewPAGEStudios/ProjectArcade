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

    public bool crouching = false;
    public bool fireHolding = false;
    public bool skillMenu = false;

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

        inputActions.playerMap.Crouch.performed += CrouchPerformed;
        inputActions.playerMap.Crouch.canceled += CrouchCanceled;

        inputActions.handMap.FireAuto.performed += FirePerformed;
        inputActions.handMap.FireAuto.canceled += FireCanceled;

        inputActions.handMap.ChangeSkill.performed += CSPerformed;
        inputActions.handMap.ChangeSkill.canceled += CSCanceled;

    }

    private void CSCanceled(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        skillMenu = false;
    }

    private void CSPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        skillMenu = true;
    }

    private void FirePerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        fireHolding = true;
    }
    private void FireCanceled(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        fireHolding = false;
    }


    private void CrouchPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        crouching = true;
    }
    private void CrouchCanceled(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        crouching = false;
    }


    public bool getCrouch()
    {
        return crouching;
    }


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

    //HandActionMaps

    public bool getReloadPressed()
    {
        return inputActions.handMap.WeaponReload.triggered;
    }

    public bool getFirePressed()
    {
        return inputActions.handMap.FireSemi.triggered;
    }
    public float getMouseScroll()
    {
        return inputActions.handMap.Scroll.ReadValue<float>();
    }


    public bool getEscapePressed()
    {
        return inputActions.GameControllerMap.backSpace.triggered;
    }
    public bool getInterractPressed()
    {
        return inputActions.GameControllerMap.Interract.triggered;
    }
}