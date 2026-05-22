using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human_Born_State : Istate
{
    public Player rp_pl;
    public Player_AI ap_pl;
    public Animator am;
    public Rigidbody2D rb;
    public Human_Born_State(Player pl)
    {
        rp_pl = pl;
        am = pl.am;
        rb = pl.rb;
    }
    public Human_Born_State(Player_AI pl)
    {
        ap_pl = pl;
        am = pl.am;
        rb = pl.rb;
    }
    public void OnEnter()
    {
        am.Play("Player_born");
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
    }

    public void OnUpdate()
    {
        if (rp_pl != null && rp_pl.pv != null && !rp_pl.pv.IsMine)
        {
            return;
        }
        if (ap_pl)
        {
            if (!ap_pl.IsDown && ap_pl.Player_Alive)
            {
                ap_pl.Transition_State(Player_State_Type.walk);
            }
        }
        else if (rp_pl)
        {
            if (!rp_pl.IsDown && rp_pl.Player_Alive)
            {
                rp_pl.Transition_State(Player_State_Type.idle);
            }
        }
    }
}
