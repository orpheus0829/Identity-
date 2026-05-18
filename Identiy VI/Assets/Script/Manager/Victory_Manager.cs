using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Victory_Manager : MonoBehaviour
{
    public static Victory_Manager instance { private set; get; }
    public event Action On_Escape;
    public event Action On_Dead;
    public int Escape_Member;
    public int Dead_Member;
    public GameObject Result_Panel;
    public TextMeshProUGUI Result_Text;
    [Range(1, 5)] public int Target_Escape_Member;
    [Range(0, 5)] public int Target_Dead_Member;
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
        Result_Panel.gameObject.SetActive(false);
        Escape_Member = 0;
        Dead_Member = 0;
    }
    public void Start()
    {
        //Target_Escape_Member = Mathf.Max(Game_Settings.instance.AI_Player_Num / 2, 1);
        //Target_Dead_Member = Game_Settings.instance.AI_Player_Num * 3 / 4;
        //if (Game_Settings.instance.AI_Player_Num <= 1)
        //{
        //    Target_Dead_Member = 1;
        //}
        int p = FindObjectsOfType<Player>().Length;
        int ai = FindObjectsOfType<Player_AI>().Length;
        int total = p + ai;
        Target_Escape_Member = Mathf.Max(total / 2, 1);
        Target_Dead_Member = Mathf.Max(total - Target_Escape_Member, 1);
    }
    public void OnEnable()
    {
        On_Escape += Escape_Member_Plus;
        On_Escape += Member_Check;
        On_Dead += Dead_Mmeber_Plus;
        On_Dead += Member_Check;
    }
    public void OnDisable()
    {
        On_Escape -= Escape_Member_Plus;
        On_Escape -= Member_Check;
        On_Dead -= Dead_Mmeber_Plus;
        On_Dead -= Member_Check;
    }
    public void Escape()
    {
        On_Escape?.Invoke();
    }
    public void Escape_Member_Plus()
    {
        Escape_Member++;
    }
    public void Dead()
    {
        On_Dead?.Invoke();
    }
    public void Dead_Mmeber_Plus()
    {
        Dead_Member++;
    }
    public void Member_Check()
    {
        if (Escape_Member >= Target_Escape_Member)
        {
            Clear_Role();
            Result_Panel.gameObject.SetActive(true);
            Result_Text.text = "Human Win";
            Debug.Log("人类胜利");
        }
        if (Dead_Member >= Target_Dead_Member)
        {
            Clear_Role();
            Result_Panel.gameObject.SetActive(true);
            Result_Text.text = "Butcher Win";
            Debug.Log("屠夫胜利");
        }
    }
    public void Clear_Role()
    {
        Cursor.visible = true;
        Animator[] Roles_Garbage = FindObjectsByType<Animator>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach(var i in Roles_Garbage)
        {
            Destroy(i.gameObject);
        }
        Sound_Manager.instance.Stop_sfx(Sound_Manager.instance.Dash_Sound);
        Sound_Manager.instance.Stop_sfx(Sound_Manager.instance.Hurt_Sound);
    }
}
