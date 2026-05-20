using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Become_Who : MonoBehaviour
{
    public Button Human_Button;
    public Button Butcher_Button;
    public GameObject Loading_Start;
    public GameObject AI_Chocie;
    public void Start()
    {
        Human_Button.onClick.AddListener(() =>
        {
            Game_Settings.instance.Play_Role = 1;
            Loading_Start.gameObject.SetActive(true);
            Loading_Game.instance.Is_Start = true;
            Human_Button.gameObject.SetActive(false);
            Butcher_Button.gameObject.SetActive(false);
            AI_Chocie.gameObject.SetActive(false);
        });
        Butcher_Button.onClick.AddListener(() =>
        {
            Game_Settings.instance.Play_Role = 2;
            Loading_Start.gameObject.SetActive(true);
            Loading_Game.instance.Is_Start = true;
            Human_Button.gameObject.SetActive(false);
            Butcher_Button.gameObject.SetActive(false);
            AI_Chocie.gameObject.SetActive(false);
        });
    }
}
