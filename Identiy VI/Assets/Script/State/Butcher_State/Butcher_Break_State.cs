using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Butcher_Break_State : Istate
{
    public Butcher rb_bt;
    public Butcher_AI ab_bt;
    public Animator am;
    public Rigidbody2D rb;
    public Butcher_Break_State(Butcher bt)
    {
        rb_bt = bt;
        am = bt.am;
        rb = bt.rb;
    }
    public Butcher_Break_State(Butcher_AI bt)
    {
        ab_bt = bt;
        am = bt.am;
        rb = bt.rb;
    }
    public void OnEnter()
    {
        if (ab_bt)
        {
            am.Play("Butcher_break");
            ab_bt.CanControl_B = false;
            rb.velocity = Vector2.zero;
        }
        if (rb_bt)
        {
            am.Play("Butcher_break");
            rb_bt.CanControl_B = false;
            rb.velocity = Vector2.zero;
        }
    }

    public void OnExit()
    {
        
    }

    public void OnFixedUpdate()
    {
        if (rb_bt != null)
        {
            if (PhotonNetwork.IsConnected && rb_bt.pv != null && !rb_bt.pv.IsMine)
                return;
        }
    }

    public void OnUpdate()
    {
        if (rb_bt != null)
        {
            if (PhotonNetwork.IsConnected && rb_bt.pv != null && !rb_bt.pv.IsMine)
                return;
        }
        if (ab_bt)
        {
            if (am.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                ab_bt.Transition_State_B(Butcher_State_Type.idle);
            }
        }
        if (rb_bt)
        {
            if (am.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                rb_bt.Transition_State_B(Butcher_State_Type.idle);
            }
        }
    }
}
