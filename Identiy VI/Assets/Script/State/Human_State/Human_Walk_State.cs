using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human_Walk_State : Istate
{
    public Player rp_pl;
    public Player_AI ap_pl;
    public Animator am;
    public Rigidbody2D rb;
    public Human_Walk_State(Player pl)
    {
        rp_pl = pl;
        am = pl.am;
        rb = pl.rb;
    }
    public Human_Walk_State(Player_AI pl)
    {
        ap_pl = pl;
        am = pl.am;
        rb = pl.rb;
    }
    public void OnEnter()
    {
        am.Play("Player_walk");
    }

    public void OnExit()
    {
        am.Play("Player_idle");
    }

    public void OnFixedUpdate()
    {
        if (rp_pl)
        {
            rp_pl.Move();
            if (rp_pl.CanControl && !rp_pl.Is_Dashing)
            {
                rp_pl.rb.velocity = rp_pl.inputMove * rp_pl.Move_Speed;
                //sr.flipX = inputMove.x < 0 ? true : false;
                if (rp_pl.rb.velocity == Vector2.zero)
                {
                    rp_pl.am.SetBool("IsRun", false);
                }
                else
                {
                    rp_pl.am.SetBool("IsRun", true);
                }
            }
        }
        if (ap_pl)
        {
            //if (ap_pl.Target_Cipher && Vector2.Distance(ap_pl.transform.position, ap_pl.Target_Cipher.transform.position) < ap_pl.InteractRadius)
            //{
            //    ap_pl.Interact();
            //}
            //if (rb.velocity.magnitude < 0.2f)
            //{
            //    ap_pl.Interact();
            //}
            ap_pl.Interact();
            ap_pl.Move();
            if (ap_pl.CanControl && !ap_pl.Is_Dashing)
            {
                ap_pl.rb.velocity = ap_pl.inputMove * ap_pl.Move_Speed;
                //sr.flipX = inputMove.x < 0 ? true : false;
                if (ap_pl.rb.velocity == Vector2.zero)
                {
                    ap_pl.am.SetBool("IsRun", false);
                }
                else
                {
                    ap_pl.am.SetBool("IsRun", true);
                }
            }
        }
    }

    public void OnUpdate()
    {
        if (rp_pl)
        {
            if (rp_pl.inputMove == Vector2.zero)
            {
                rp_pl.Transition_State(Player_State_Type.idle);
            }
            if (rp_pl.Player_Coding)
            {
                rp_pl.Transition_State(Player_State_Type.coding);
            }
            if (rp_pl.Is_Dashing)
            {
                rp_pl.Transition_State(Player_State_Type.dash);
            }
            if (rp_pl.CurrentHp <= 0)
            {
                rp_pl.Transition_State(Player_State_Type.down);
            }
        }
        else if (ap_pl)
        {
            if (ap_pl.pathPointList == null || ap_pl.currentIndex >= ap_pl.pathPointList.Count)
            {
                if (!ap_pl.Chasing)
                {
                    ap_pl.Transition_State(Player_State_Type.idle);
                }
            }
            if (!ap_pl.Chasing && ap_pl.Target_Cipher && Vector2.Distance(ap_pl.transform.position, ap_pl.Target_Cipher.transform.position) < ap_pl.InteractRadius && ap_pl.ciph && !ap_pl.ciph.Done)
            {
                Debug.Log("qwert.walk");
                ap_pl.Interact();
                ap_pl.Transition_State(Player_State_Type.coding);
            }
            if (ap_pl.Is_Dashing)
            {
                ap_pl.Transition_State(Player_State_Type.dash);
            }
            if (ap_pl.CurrentHp <= 0)
            {
                ap_pl.Transition_State(Player_State_Type.down);
            }
        }
    }
}
