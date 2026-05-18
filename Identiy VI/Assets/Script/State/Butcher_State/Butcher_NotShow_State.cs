using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Butcher_NotShow_State : Istate
{
    public Butcher rb_bt;
    public Butcher_AI ab_bt;
    public Animator am;
    public Rigidbody2D rb;
    public Butcher_NotShow_State(Butcher bt)
    {
        rb_bt = bt;
        am = bt.am;
        rb = bt.rb;
    }
    public Butcher_NotShow_State(Butcher_AI bt)
    {
        ab_bt = bt;
        am = bt.am;
        rb = bt.rb;
    }
    public void OnEnter()
    {
        if (ab_bt)
        {
            am.Play("Butcher_notshow");
            ab_bt.Attack_Recovery();
        }
        if (rb_bt)
        {
            am.Play("Butcher_notshow");
            rb_bt.Attack_Recovery();
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
