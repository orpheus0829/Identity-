using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Butcher Input")]
public class ButcherInput : ScriptableObject, InputActions.IButcherActions
{
    public event UnityAction<Vector2> onMove;
    public event UnityAction onInteract;
    public event UnityAction onAttack;
    public event UnityAction onEscape;
    public InputActions inputact_B;
    public InputControl pressedControl_Move;
    public InputControl pressedControl_Interact;
    public InputControl pressedControl_Attack;
    public InputControl pressedControl_Escape;
    private void OnEnable()
    {
        inputact_B = new InputActions();
        inputact_B.Butcher.SetCallbacks(this);
    }
    public void OnDisable()
    {
        if (inputact_B != null)
        {
            inputact_B.Butcher.Disable();
            inputact_B.Disable();
        }
    }
    public void SwitchActionMap(InputActionMap actionMap)
    {
        inputact_B.Disable();
        actionMap.Enable();
    }
    public void EnabaleButcherInput()
    {
        inputact_B.Butcher.Enable();
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        pressedControl_Move = context.control;
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                onMove?.Invoke(context.ReadValue<Vector2>());
                break;
            case InputActionPhase.Canceled:
                onMove?.Invoke(Vector2.zero);
                break;
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        pressedControl_Interact = context.control;
        if (context.started)
        {
            onInteract?.Invoke();
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        pressedControl_Attack = context.control;
        if (context.started)
        {
            onAttack?.Invoke();
        }
    }
    public void OnESC(InputAction.CallbackContext context)
    {
        pressedControl_Escape = context.control;
        if (context.started)
        {
            onEscape?.Invoke();
        }
    }

}
