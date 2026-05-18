using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="HumanData",menuName="Data/HumanData")]
public class HumanData : ScriptableObject
{
    [Header(" Ù–‘")]
    public string top_name;
    [Range(0f,100f)]public float MaxHp;
    public float Speed;
    public float ActSpeed_Cross;
    public float ActSpeed_DropBoard;
    public float CodeSpeed;
    [Range(0f,10f)]public float Interact_Radius;
    [Range(0f, 50f)] public float Dashing_Force;
    [Range(0f, 10f)] public float Speed_Up_When_Hurt;
    [Range(0f, 1f)] public float Speed_Up_Time;
    [Range(0f, 120f)] public float Aid_Time;
    [Range(0, 10)] public int Saving_Chance;
    [Range(0f, 100f)] public float Dash_Cool;
    [Range(0f, 100f)] public float Heart_Beat_radius;
    [Range(0f, 5f)] public float Dash_Duration;
    [Range(0f, 20f)] public float Dash_CD;
    //[Range(0f,500f)]public float Med_Kit;
}
