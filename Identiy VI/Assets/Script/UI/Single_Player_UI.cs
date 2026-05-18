using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Single_Player_UI : MonoBehaviour
{
    [Header("身份")]
    [SerializeField]public string name_Const;
    [Header("UI引用")]
    public TextMeshProUGUI Player_Name;
    public Image Slider_Cover;
    public void Init(string Name)
    {
        Player_Name.text = Name;
        name_Const = Name;
        Slider_Cover.fillAmount = 0f;
    }
    public void Update_HP(float Current_HP)
    {
        Slider_Cover.fillAmount = Mathf.Clamp01(Current_HP);
    }
}
