using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ButcherData", menuName = "Data/ButcherData")]
public class ButcherData : ScriptableObject
{
    [Header("ÊôĞÔ")]
    public string top_name;
    [Range(0f, 100f)] public float Damage;
    public float StunTime;
    [Range(0, 10f)] public float Interact_Radius;
    public float Speed;
    public float ActSpeed_Cross;
    public float ActSpeed_BreakBoard;
    [Header("¹¥»÷¾àÀë")]
    [Range(0f, 10f)] public float Attack_Distance;
    [Header("¹¥»÷ºóÒ¡")]
    [Range(0f,10f)] public float Attack_Stun_Time;
}
