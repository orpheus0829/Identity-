using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Player Input")]
public class PlayerInput : ScriptableObject,InputActions.IHumanActions
{
    public event UnityAction<Vector2> onMove;
    public event UnityAction onInteract;
    public event UnityAction onSkill;
    public event UnityAction onEscape;
    //public Animator am;
    //public Player pl;
    public InputActions inputact;
    public InputControl pressedControl_Move;
    public InputControl pressedControl_Interact;
    public InputControl pressedControl_Skill;
    public InputControl pressedControl_Escape;
    private void OnEnable()
    {
        inputact = new InputActions();
        inputact.Human.SetCallbacks(this);
    }
    public void SwitchActionMap(InputActionMap actionMap)
    {
        if (inputact == null)
        {
            inputact = new InputActions();
            inputact.Human.SetCallbacks(this);
        }
        inputact.Disable();
        actionMap.Enable();
    }
    public void ResetHumanInput()
    {
        inputact.Disable();
        inputact.Human.SetCallbacks(this);
        inputact.Human.Enable();
    }
    public void EnabaleHumanInput()
    {
        inputact.Human.Enable();
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

    public void OnSkill(InputAction.CallbackContext context)
    {
        pressedControl_Skill = context.control;
        if (context.started)
        {
            onSkill?.Invoke();
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
