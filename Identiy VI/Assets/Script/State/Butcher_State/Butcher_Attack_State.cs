using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Butcher_Attack_State : Istate
{
    public Butcher rb_bt;
    public Butcher_AI ab_bt;
    public Animator am;
    public Rigidbody2D rb;
    public Butcher_Attack_State(Butcher bt)
    {
        rb_bt = bt;
        am = bt.am;
        rb = bt.rb;
    }
    public Butcher_Attack_State(Butcher_AI bt)
    {
        ab_bt = bt;
        am = bt.am;
        rb = bt.rb;
    }
    public void OnEnter()
    {
        am.Play("Butcher_attack");
    }

    public void OnExit()
    {
        
    }

    public void OnFixedUpdate()
    {
        if (rb_bt != null && rb_bt.pv != null && !rb_bt.pv.IsMine)
        {
            return;
        }
        if (ab_bt)
        {
            ab_bt.Move();
            if (ab_bt.CanControl_B)
            {
                ab_bt.rb.velocity = ab_bt.inputMove_B * ab_bt.Speed_B;
            }
        }
        if (rb_bt)
        {
            rb_bt.Move();
            if (rb_bt.CanControl_B)
            {
                rb_bt.rb.velocity = rb_bt.inputMove_B * rb_bt.Speed_B;
            }
        }
    }

    public void OnUpdate()
    {
        if (rb_bt != null && rb_bt.pv != null && !rb_bt.pv.IsMine)
        {
            return;
        }
        if (ab_bt)
        {
            if (am.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                if (ab_bt.collider_Human)
                {
                    rb.mass = 9999;
                    ab_bt.Transition_State_B(Butcher_State_Type.show);
                }
                else
                {
                    ab_bt.Transition_State_B(Butcher_State_Type.notshow);
                }
                ab_bt.collider_Human = null;
            }
        }
        if (rb_bt)
        {
            if (am.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                if (rb_bt.collider_Human)
                {
                    rb.mass = 9999;
                    rb_bt.Transition_State_B(Butcher_State_Type.show);
                }
                else
                {
                    rb_bt.Transition_State_B(Butcher_State_Type.notshow);
                }
                rb_bt.collider_Human = null;
            }
        }
    }
}
