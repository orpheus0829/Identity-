using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player_UI : MonoBehaviour
{
    public  static Player_UI instance { private set; get; }
    public Player pl;
    public TextMeshProUGUI Count_CD;
    public Image Dash_Cover;
    public bool Is_CD;
    public void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);

        }
        Skill_Reset();
        Is_CD = false;
    }
    public void Update()
    {
        if (Is_CD && pl.Dash_Cooldown > 0)
        {
            Count_CD.gameObject.SetActive(true);
            Dash_Cover.fillAmount = pl.Dash_Cooldown / pl.Dash_CD;
            if (pl.Dash_Cooldown >= 1)
            {
                Count_CD.text = $"{Mathf.CeilToInt(pl.Dash_Cooldown)}";
            }
            else
            {
                Count_CD.text = $"{pl.Dash_Cooldown.ToString("F1")}";
            }
            if (Is_CD && pl.Dash_Cooldown <= 0.1f)
            {
                Skill_Reset();
            }
        }
        //if (Is_CD && pl.Dash_Cooldown ==0.01f)
        //{
        //    Is_CD = false;
        //    Count_CD.gameObject.SetActive(false);
        //    Skill_Reset();
        //}
    }
    public void Dash_Start()
    {
        Is_CD = true;
        Dash_Cover.fillAmount = 1f;
    }
    public void Skill_Reset()
    {
        Count_CD.text = string.Empty;
        Count_CD.gameObject.SetActive(false);
        Dash_Cover.fillAmount = 0f;
        Is_CD = false;
    }
}
