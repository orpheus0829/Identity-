using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Option_Button : MonoBehaviour
{
    public GameObject Options_Panel;
    public void Awake()
    {
        Options_Panel.SetActive(false);
    }
    public void Enter_Options()
    {
        Options_Panel.SetActive(true);
    }
}
