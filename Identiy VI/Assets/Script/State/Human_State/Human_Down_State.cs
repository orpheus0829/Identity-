using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human_Down_State : Istate
{
    public Player rp_pl;
    public Player_AI ap_pl;
    public Animator am;
    public Rigidbody2D rb;
    public Human_Down_State(Player pl)
    {
        rp_pl = pl;
        am = pl.am;
        rb = pl.rb;
    }
    public Human_Down_State(Player_AI pl)
    {
        ap_pl = pl;
        am = pl.am;
        rb = pl.rb;
    }
    public void OnEnter()
    {
        am.Play("Player_down");
    }

    public void OnExit()
    {
        
    }

    public void OnFixedUpdate()
    {
        
    }

    public void OnUpdate()
    {
        if (ap_pl)
        {
            ap_pl.Dying_Time = ap_pl.SelfSaving_Chance == 0 ? 0 : ap_pl.Dying_Time;
            if (am.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                ap_pl.Transition_State(Player_State_Type.needhelp);
            }
        }
        else if (rp_pl)
        {
            rp_pl.Dying_Time = rp_pl.SelfSaving_Chance == 0 ? 0 : rp_pl.Dying_Time;
            if (am.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                rp_pl.Transition_State(Player_State_Type.needhelp);
            }
        }
    }
}
