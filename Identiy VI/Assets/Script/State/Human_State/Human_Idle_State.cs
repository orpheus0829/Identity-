using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Human_Idle_State : Istate
{
    public Player rp_pl;
    public Player_AI ap_pl;
    public Animator am;
    public Rigidbody2D rb;
    public Human_Idle_State(Player pl)
    {
        rp_pl = pl;
        am = pl.am;
        rb = pl.rb;
    }
    public Human_Idle_State(Player_AI pl)
    {
        ap_pl = pl;
        am = pl.am;
        rb = pl.rb;
    }
    public void OnEnter()
    {
        am.Play("Player_idle");
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
        am.SetBool("IsRun", false);
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
            if (ap_pl)
        {
            if (ap_pl.Target_Cipher && Vector2.Distance(ap_pl.transform.position, ap_pl.Target_Cipher.transform.position) < ap_pl.InteractRadius)
            {
                ap_pl.Interact();
            }
        }
    }

    public void OnUpdate()
    {
        if (rp_pl != null && rp_pl.pv != null && !rp_pl.pv.IsMine)
        {
            return;
        }
        if (rp_pl)
        {
            if (rp_pl.inputMove != Vector2.zero)
            {
                rp_pl.Transition_State(Player_State_Type.walk);
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
                return;
            }
        }
        else if (ap_pl)
        {
            if (!ap_pl.Chasing && ap_pl.pathPointList != null && ap_pl.currentIndex < ap_pl.pathPointList.Count)
            {
                ap_pl.Transition_State(Player_State_Type.walk);
            }
            if (!ap_pl.Chasing && ap_pl.Target_Cipher && Vector2.Distance(ap_pl.transform.position, ap_pl.Target_Cipher.transform.position) < ap_pl.InteractRadius && ap_pl.ciph && !ap_pl.ciph.Done)
            {
                Debug.Log("qwert.idle");
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
