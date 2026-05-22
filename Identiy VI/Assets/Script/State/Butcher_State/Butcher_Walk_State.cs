using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Butcher_Walk_State : Istate
{
    public Butcher rb_bt;
    public Butcher_AI ab_bt;
    public Animator am;
    public Rigidbody2D rb;
    public Butcher_Walk_State(Butcher bt)
    {
        rb_bt = bt;
        am = bt.am;
        rb = bt.rb;
    }
    public Butcher_Walk_State(Butcher_AI bt)
    {
        ab_bt = bt;
        am = bt.am;
        rb = bt.rb;
    }
    public void OnEnter()
    {
        am.Play("Butcher_walk");
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
        if (ab_bt)
        {
            if (Vector2.Distance(ab_bt.transform.position, ab_bt.Target_Human.transform.position) < ab_bt.Attack_Distance_B)
            {
                Vector2 dir = ab_bt.Target_Human.transform.position - ab_bt.transform.position;
                RaycastHit2D hit = Physics2D.Raycast(ab_bt.transform.position, dir.normalized, Vector2.Distance(ab_bt.transform.position, ab_bt.Target_Human.transform.position), ab_bt.Wall_Layer);
                if (!hit.collider)
                {
                    ab_bt.Attack();
                }
            }
            if (rb.velocity.magnitude <= 0.2f)
            {
                ab_bt.Interact();
            }
            ab_bt.Move();
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
        if (rb_bt != null)
        {
            if (PhotonNetwork.IsConnected && rb_bt.pv != null && !rb_bt.pv.IsMine)
                return;
        }
        if (ab_bt)
        {
            if (ab_bt.pathPointList == null || ab_bt.currentIndex >= ab_bt.pathPointList.Count)
            {
                ab_bt.Transition_State_B(Butcher_State_Type.idle);
            }
            if (ab_bt.Is_Attack_B)
            {
                ab_bt.Transition_State_B(Butcher_State_Type.attack);
            }
            if (ab_bt.board && ab_bt.breaking)
            {
                ab_bt.Transition_State_B(Butcher_State_Type.breaking);
            }
            if (ab_bt.Is_Stun_B)
            {
                ab_bt.Transition_State_B(Butcher_State_Type.stun);
            }
        }
        if (rb_bt)
        {
            if (rb_bt.rb.velocity == Vector2.zero)
            {
                rb_bt.Transition_State_B(Butcher_State_Type.idle);
            }
            if (rb_bt.Is_Attack_B)
            {
                rb_bt.Transition_State_B(Butcher_State_Type.attack);
            }
            if (rb_bt.board && rb_bt.breaking)
            {
                rb_bt.Transition_State_B(Butcher_State_Type.breaking);
            }
            if (rb_bt.Is_Stun_B)
            {
                rb_bt.Transition_State_B(Butcher_State_Type.stun);
            }
        }
    }
}
