using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human_Coding_State : Istate
{
    public Player rp_pl;
    public Player_AI ap_pl;
    public Animator am;
    public Rigidbody2D rb;
    public Human_Coding_State(Player pl)
    {
        rp_pl = pl;
        am = pl.am;
        rb = pl.rb;
    }
    public Human_Coding_State(Player_AI pl)
    {
        ap_pl = pl;
        am = pl.am;
        rb = pl.rb;
    }
    public void OnEnter()
    {
        am.Play("Player_coding");
        rb.velocity = Vector2.zero;
        if (ap_pl)
        {
            ap_pl.CanControl = false;
            ap_pl.Coding_Enter();
            ap_pl.Player_Coding = true;
        }
        if (rp_pl)
        {
            rp_pl.Player_Coding = true;
        }
    }

    public void OnExit()
    {
        if (ap_pl)
        {
            ap_pl.Coding_Exit();
            ap_pl.Player_Coding = false;
            rb.velocity = Vector2.zero;
            if (ap_pl.ciph != null)
            {
                ap_pl.ciph.is_coding = false;
            }
        }
        if (rp_pl)
        {
            rp_pl.Player_Coding = false;
            rb.velocity = Vector2.zero;
            if (rp_pl.ciph != null)
            {
                rp_pl.ciph.is_coding = false;
            }
        }
    }

    public void OnFixedUpdate()
    {
        if (ap_pl)
        {
            //ap_pl.Interact();
            ap_pl.Coding_Check();
        }
        if (rp_pl)
        {
            rp_pl.Coding_Check();
        }
    }

    public void OnUpdate()
    {
        if (ap_pl)
        {
            if (ap_pl.ciph && ap_pl.ciph.Done)
            {
                ap_pl.Player_Coding = false;
                ap_pl.ciph = null;
                ap_pl.Transition_State(Player_State_Type.idle);
            }
            if (ap_pl.ciph && ap_pl.pathPointList != null && ap_pl.currentIndex < ap_pl.pathPointList.Count && ap_pl.ciph.Done)
            {
                ap_pl.Coding_Exit();
                ap_pl.Transition_State(Player_State_Type.walk);
            }
            if (ap_pl.Is_Dashing)
            {
                ap_pl.Transition_State(Player_State_Type.dash);
            }
            if (ap_pl.CurrentHp <= 0)
            {
                ap_pl.Transition_State(Player_State_Type.down);
            }
            if (ap_pl.IsHurt && ap_pl.ciph.Done)
            {
                ap_pl.Coding_Exit();
                ap_pl.ciph = null;
                ap_pl.Transition_State(Player_State_Type.idle);
            }
        }
        else if (rp_pl)
        {
            if (rp_pl.inputMove != Vector2.zero || rp_pl.ciph.Done)
            {
                rp_pl.Coding_Exit();
                rp_pl.Transition_State(Player_State_Type.walk);
            }
            if (rp_pl.Is_Dashing)
            {
                rp_pl.Transition_State(Player_State_Type.dash);
            }
            if (rp_pl.CurrentHp <= 0)
            {
                rp_pl.Transition_State(Player_State_Type.down);
            }
            if (rp_pl.IsHurt)
            {
                rp_pl.Coding_Exit();
                rp_pl.Transition_State(Player_State_Type.idle);
            }
        }
    }
}
