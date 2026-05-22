using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Window : MonoBehaviourPunCallbacks
{
    [Header("贴图")]
    public List<Sprite> sprites;
    [Header("落点")]
    [Range(0f,20f)]public float away;
    public Vector2 Up_Location;
    public Vector2 Down_Loction;
    [Header("冷却")]
    public bool _Using;
    public bool Ban;
    public float Frezzing_Duration;
    public float Frezzing_Count;
    [Header("PUN2")]
    public PhotonView pv;
    [Header("引用")]
    public SpriteRenderer sr;
    public void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        pv = GetComponent<PhotonView>();

        sr.sprite = sprites[Random.Range(0, sprites.Count - 1)];
        Up_Location = new Vector2(transform.position.x, transform.position.y + 0.5f + away);
        Down_Loction = new Vector2(transform.position.x, transform.position.y + 0.5f - away);
    }
    public void Update()
    {
        if (Ban)
        {
            Frezzing_Count -= Time.deltaTime;
            if (Frezzing_Count <= 0)
            {
                Ban = false;
            }
        }
    }
    #region 交互
    public void Interact_Window_Player_Real(Player p)
    {
        if (pv != null && !p.photonView.IsMine)
        {
            return;
        }
        p.Landing = p.transform.position.y < transform.position.y ? Up_Location : Down_Loction;
        p.capsuleCollider2D.enabled = false;
        p.CanControl = false;
        p.Move_Speed = 0;
        p.rb.mass = 9999;
        p.Crossing = true;
        p.transform.DOMove(p.Landing, 0.5f).OnComplete(() =>
        {
            p.Crossing = false;
            p.CanControl = true;
            p.rb.velocity = Vector2.zero;
            p.capsuleCollider2D.enabled = true;
            p.Move_Speed = p.human_data.Speed;
            p.rb.mass = 1;
        });
        return;
    }
    public void Interact_Window_Player_AI(Player_AI ai)
    {
        if (Ban)
        {
            return;
        }
        ai.Landing = ai.transform.position.y < transform.position.y ? Up_Location : Down_Loction;
        ai.capsuleCollider2D.enabled = false;
        ai.CanControl = false;
        ai.Move_Speed = 0;
        ai.rb.mass = 9999;
        ai.Crossing = true;
        ai.transform.DOMove(ai.Landing, 0.5f).OnComplete(() =>
        {
            ai.Crossing = false;
            ai.CanControl = true;
            ai.rb.velocity = Vector2.zero;
            ai.capsuleCollider2D.enabled = true;
            ai.Move_Speed = ai.human_data.Speed;
            ai.rb.mass = 1;
        });
        Ban = true;
        Frezzing_Count = Frezzing_Duration;
        return;
    }
    public void Interact_Window_Butcher_Real(Butcher b)
    {
        if (pv != null && !b.photonView.IsMine)
        {
            return;
        }
        b.Landing_B = b.transform.position.y < transform.position.y ? Up_Location : Down_Loction;
        b.col.enabled = false;
        b.CanControl_B = false;
        b.Speed_B = 0;
        b.rb.mass = 9999;
        b.Crossing_B = true;
        b.transform.DOMove(b.Landing_B, 0.5f).OnComplete(() =>
        {
            b.Crossing_B = false;
            b.CanControl_B = true;
            b.rb.velocity = Vector2.zero;
            b.col.enabled = true;
            b.Speed_B = b.butcher_data.Speed;
            b.rb.mass = 1;
        });
        return;
    }
    public void Interact_Window_Butcher_AI(Butcher_AI ai)
    {
        if (Ban)
        {
            return;
        }
        ai.Landing_B = ai.transform.position.y < transform.position.y ? Up_Location : Down_Loction;
        ai.col.enabled = false;
        ai.CanControl_B = false;
        ai.Speed_B = 0;
        ai.rb.mass = 9999;
        ai.Crossing_B = true;
        ai.transform.DOMove(ai.Landing_B, 0.5f).OnComplete(() =>
        {
            ai.Crossing_B = false;
            ai.CanControl_B = true;
            ai.rb.velocity = Vector2.zero;
            ai.col.enabled = true;
            ai.Speed_B = ai.butcher_data.Speed;
            ai.rb.mass = 1;
        });
        Ban = true;
        Frezzing_Count = Frezzing_Duration;
        return;
    }
    #endregion
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(Up_Location, 0.1f);
        Gizmos.DrawSphere(Down_Loction, 0.1f);
    }
}
