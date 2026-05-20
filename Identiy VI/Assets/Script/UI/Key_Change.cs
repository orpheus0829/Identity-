using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Key_Change : MonoBehaviour
{
    public InputActionReference key_ref;
    public PlayerInput pl_input;
    public TextMeshProUGUI Key_Text;
    public void Start()
    {
        Refresh_Key_Name();
    }
    public void Change_Key()
    {
        InputAction action = key_ref.action;
        action.Disable();
        Key_Text.text = "«Î ‰»Î∞¥º¸";
        action.PerformInteractiveRebinding(0).WithCancelingThrough("<Keyboard>/escape").OnComplete(rebindOp =>
        {
            string newPath = rebindOp.selectedControl.path;
            action.ApplyBindingOverride(0, newPath);
            rebindOp.Dispose();
            action.Enable();
            Refresh_Key_Name();
        }).OnCancel(rebindOp =>
        {
            rebindOp.Dispose();
            action.Enable();
            Refresh_Key_Name();
        }).Start();
    }
    public void Refresh_Key_Name()
    {
        if (key_ref == null || key_ref.action == null || key_ref.action.bindings.Count == 0)
        {
            Key_Text.text = "Œ¥∞Û∂®";
            return;
        }
        string bindingPath = key_ref.action.bindings[0].effectivePath;
        string Readable_Key_Name = InputControlPath.ToHumanReadableString(bindingPath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        Key_Text.text = Readable_Key_Name;
    }
}
