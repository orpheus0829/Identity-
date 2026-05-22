using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human_Dash_State : Istate
{
    public Player rp_pl;
    public Player_AI ap_pl;
    public Animator am;
    public Rigidbody2D rb;
    public Human_Dash_State(Player pl)
    {
        rp_pl = pl;
        am = pl.am;
        rb = pl.rb;
    }
    public Human_Dash_State(Player_AI pl)
    {
        ap_pl = pl;
        am = pl.am;
        rb = pl.rb;
    }
    public void OnEnter()
    {
        am.Play("Player_dash");
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
            if (!ap_pl.Is_Dashing)
            {
                ap_pl.Transition_State(Player_State_Type.idle);
                ap_pl.rb.velocity = Vector2.zero;
            }
        }
        else if (rp_pl)
        {
            if (!rp_pl.Is_Dashing)
            {
                rp_pl.Transition_State(Player_State_Type.idle);
                rp_pl.rb.velocity = Vector2.zero;
            }
        }
    }
}
