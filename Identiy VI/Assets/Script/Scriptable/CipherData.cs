using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="CipherData",menuName ="Data/CipherData")]
public class CipherData : ScriptableObject
{
    [Header("属性")]
    public float Cipher_Interact_Radius;
    [Header("显示破译进度条的距离半径")]
    [Range(0f,10f)]public float Cipher_Slider_Show_Radius;
    [Header("所需破译量")]
    [Range(0f, 1000f)] public float Cipher_Need_INT;
    [Header("出现校准概率")]
    [Range(0f, 100f)] public float Calibration_Random;
}
