using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Butcher_Stun_State : Istate
{
    public Butcher rb_bt;
    public Butcher_AI ab_bt;
    public Animator am;
    public Rigidbody2D rb;
    public Butcher_Stun_State(Butcher bt)
    {
        rb_bt = bt;
        am = bt.am;
        rb = bt.rb;
    }
    public Butcher_Stun_State(Butcher_AI bt)
    {
        ab_bt = bt;
        am = bt.am;
        rb = bt.rb;
    }
    public void OnEnter()
    {
        rb.drag = 999999;
        if (ab_bt)
        {
            am.Play("Butcher_stun");
            ab_bt.Is_Stun_B = false;
            ab_bt.CanControl_B = false;
        }
        if (rb_bt)
        {
            am.Play("Butcher_stun");
            rb_bt.Is_Stun_B = false;
            rb_bt.CanControl_B = false;
        }
    }

    public void OnExit()
    {
        
    }

    public void OnFixedUpdate()
    {
        
    }

    public void OnUpdate()
    {
        if (ab_bt)
        {
            if (am.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                rb.drag = 0;
                ab_bt.CanControl_B = true;
                ab_bt.Transition_State_B(Butcher_State_Type.idle);
            }
        }
        if (rb_bt)
        {
            if (am.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                rb.drag = 0;
                rb_bt.CanControl_B = true;
                rb_bt.Transition_State_B(Butcher_State_Type.idle);
            }
        }
    }
}
