using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Change_AI_Num : MonoBehaviour
{
    public TMP_InputField Human;
    public TMP_InputField Butcher;
    public int min = 0;
    public int Human_max = 10;
    public int Butcher_max = 4;
    public void Awake()
    {
        Human.text = min.ToString();
        Butcher.text = min.ToString();
    }
    public void Start()
    {
        Human.onEndEdit.AddListener(text =>
        {
            if (string.IsNullOrEmpty(text))
            {
                Human.text = min.ToString();
                return;
            }
            if (int.TryParse(text, out int value))
            {
                value = Mathf.Clamp(value, min, Human_max);
                Human.text = value.ToString();
            }
            else
            {
                Human.text = min.ToString();
            }
        });
        Butcher.onEndEdit.AddListener(text =>
        {
            if (string.IsNullOrEmpty(text))
            {
                Butcher.text = min.ToString();
                return;
            }
            if (int.TryParse(text, out int value))
            {
                value = Mathf.Clamp(value, min, Butcher_max);
                Butcher.text = value.ToString();
            }
            else
            {
                Butcher.text = min.ToString();
            }
        });
    }
    public void Update()
    {
        Game_Settings.instance.AI_Player_Num = int.Parse(Human.text);
        Game_Settings.instance.AI_Buthcer_Num = int.Parse(Butcher.text);
    }
}
