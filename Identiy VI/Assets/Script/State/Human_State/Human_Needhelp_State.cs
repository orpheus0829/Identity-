using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human_Needhelp_State : Istate
{
    public Player rp_pl;
    public Player_AI ap_pl;
    public Animator am;
    public Rigidbody2D rb;
    public Human_Needhelp_State(Player pl)
    {
        rp_pl = pl;
        am = pl.am;
        rb = pl.rb;
    }
    public Human_Needhelp_State(Player_AI pl)
    {
        ap_pl = pl;
        am = pl.am;
        rb = pl.rb;
    }
    public void OnEnter()
    {
        am.Play("Player_needhelp");
        if (ap_pl && ap_pl.SelfSaving_Chance > 0)
        {
            ap_pl.Be_Saving = true;
        }
    }

    public void OnExit()
    {
        
    }

    public void OnFixedUpdate()
    {
        if (rp_pl != null && rp_pl.pv != null && !rp_pl.pv.IsMine)
        {
            return;
        }
        if (ap_pl)
        {
            ap_pl.Saving_Check();
        }
        if (rp_pl)
        {
            rp_pl.Saving_Check();
        }
    }

    public void OnUpdate()
    {
        if (rp_pl != null && rp_pl.pv != null && !rp_pl.pv.IsMine)
        {
            return;
        }
        if (ap_pl)
        {
            ap_pl.Get_Aid();
        }
        else if (rp_pl)
        {
            rp_pl.Get_Aid();
        }
    }
}