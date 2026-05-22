using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Butcher_Show_State : Istate
{
    public Butcher rb_bt;
    public Butcher_AI ab_bt;
    public Animator am;
    public Rigidbody2D rb;
    public Butcher_Show_State(Butcher bt)
    {
        rb_bt = bt;
        am = bt.am;
        rb = bt.rb;
    }
    public Butcher_Show_State(Butcher_AI bt)
    {
        ab_bt = bt;
        am = bt.am;
        rb = bt.rb;
    }
    public void OnEnter()
    {
        rb.drag = 9999999;
        if (ab_bt)
        {
            am.Play("Butcher_show");
            ab_bt.Speed_B *= (float)0.7;
        }
        if (rb_bt)
        {
            am.Play("Butcher_show");
            rb_bt.Speed_B *= (float)0.7;
        }
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
                rb.drag = 0;
                ab_bt.Speed_B = ab_bt.butcher_data.Speed;
                rb.mass = 1;
                ab_bt.Transition_State_B(Butcher_State_Type.idle);
            }
        }
        if (rb_bt)
        {
            if (am.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                rb.drag = 0;
                rb_bt.Speed_B = rb_bt.butcher_data.Speed;
                rb.mass = 1;
                rb_bt.Transition_State_B(Butcher_State_Type.idle);
            }
        }
    }
}
