using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum Board_Style
{
    Normal,
    Down,
    Broken,
}
public class Board : MonoBehaviourPunCallbacks
{
    [Header("ÔÒÈË°ë¾¶")]
    [Range(0f, 10f)] public float Hit_Radius;
    [Header("×´Ì¬ÌùÍ¼")]
    public Sprite Original_Style;
    public Sprite Lay_Down_Style;
    public Sprite Tell_Apart__Style;
    [Header("×´Ì¬")]
    public Board_Style Current_State;
    [Header("Âäµã")]
    [Range(0f, 20f)] public float away;
    public Vector2 Up_Location;
    public Vector2 Down_Loction;
    [Header("ÀäÈ´")]
    public bool _Using;
    public bool Ban;
    public float Frezzing_Duration;
    public float Frezzing_Count;
    [Header("PUN2")]
    public PhotonView pv;
    [Header("ÒýÓÃ")]
    public SpriteRenderer sr;
    public BoxCollider2D col;
    public void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
        sr.sprite = Original_Style;
        pv = GetComponent<PhotonView>();

        Up_Location = new Vector2(transform.position.x, transform.position.y - 0.25f + away);
        Down_Loction = new Vector2(transform.position.x, transform.position.y - 0.25f - away);
    }
    public void Start()
    {
        Change_State(Board_Style.Normal);
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
    public void FixedUpdate()
    {

    }
    [PunRPC]
    public void RPC_UpdateState(int stateNum)
    {
        Current_State = (Board_Style)stateNum;
        switch (Current_State)
        {
            case Board_Style.Normal:
                Normal_Style();
                break;
            case Board_Style.Down:
                Down_Style();
                break;
            case Board_Style.Broken:
                Broken_Style();
                break;
        }
    }
    public void Change_State(Board_Style Now_State)
    {
        Current_State = Now_State;
        if (pv != null && pv.IsMine)
        {
            pv.RPC("RPC_UpdateState", RpcTarget.AllBuffered, (int)Now_State);
        }
        switch (Current_State)
        {
            case Board_Style.Normal:
                Normal_Style();
                break;
            case Board_Style.Down:
                Down_Style();
                break;
            case Board_Style.Broken:
                Broken_Style();
                break;
            default:
                break;
        }
    }
    public void Normal_Style()
    {
        sr.sprite = Original_Style;
        col.isTrigger = true;
    }
    public void Down_Style()
    {
        sr.sprite = Lay_Down_Style;
        col.isTrigger = false;
    }
    public void Broken_Style()
    {
        sr.sprite = Tell_Apart__Style;
        col.isTrigger = true;
        sr.sortingOrder = 4;
        gameObject.tag = "Broken_Board";
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
    #region ½»»¥
    public void Interact_Board_Player_Real(Player pl)
    {
        if (pv != null && !pl.photonView.IsMine)
        {
            return;
        }
        Butcher butcher;
        Butcher_AI butcher_ai;
        if (Current_State == Board_Style.Normal)
        {
            Change_State(Board_Style.Down);
            pl.StartCoroutine(nameof(pl.Drop_Frezze));
            Collider2D[] Collider_Hit = Physics2D.OverlapCircleAll(transform.position, Hit_Radius);
            foreach (var colliderhit in Collider_Hit)
            {
                if (colliderhit.gameObject == pl.gameObject)
                {
                    continue;
                }
                if (colliderhit && colliderhit.tag == "Butcher")
                {
                    butcher = colliderhit.GetComponent<Butcher>();
                    butcher.transform.position += butcher.transform.position.y > transform.position.y ? new Vector3(0, 1, 0) : new Vector3(0, -1, 0);
                    butcher.When_Stun();
                    break;
                }
                if (colliderhit && colliderhit.tag == "Butcher_Bot")
                {
                    butcher_ai = colliderhit.GetComponent<Butcher_AI>();
                    butcher_ai.transform.position += butcher_ai.transform.position.y > transform.position.y ? new Vector3(0, 1, 0) : new Vector3(0, -1, 0);
                    butcher_ai.When_Stun();
                    break;
                }
            }
            return;
        }
        if (Current_State == Board_Style.Down)
        {
            pl.Landing = pl.transform.position.y < transform.position.y ? Up_Location : Down_Loction;
            pl.capsuleCollider2D.enabled = false;
            pl.CanControl = false;
            pl.Move_Speed = 0;
            pl.Crossing = true;
            pl.transform.DOMove(pl.Landing, 0.5f).OnComplete(() =>
            {
                pl.Crossing = false;
                pl.CanControl = true;
                pl.rb.velocity = Vector2.zero;
                pl.capsuleCollider2D.enabled = true;
                pl.Move_Speed = pl.human_data.Speed;
                pl.rb.mass = 1;
            });
            return;
        }
    }
    public void Interact_Board_Player_AI(Player_AI ai)
    {
        if (Ban)
        {
            return;
        }
        Butcher butcher;
        Butcher_AI butcher_ai;
        if (Current_State == Board_Style.Normal)
        {
            Change_State(Board_Style.Down);
            ai.StartCoroutine(nameof(ai.Drop_Frezze));
            Collider2D[] Collider_Hit = Physics2D.OverlapCircleAll(transform.position, Hit_Radius);
            foreach (var colliderhit in Collider_Hit)
            {
                if (colliderhit.gameObject == ai.gameObject)
                {
                    continue;
                }
                if (colliderhit && colliderhit.tag == "Butcher")
                {
                    butcher = colliderhit.GetComponent<Butcher>();
                    butcher.transform.position += butcher.transform.position.y > transform.position.y ? new Vector3(0, 1, 0) : new Vector3(0, -1, 0);
                    butcher.When_Stun();
                    break;
                }
                if (colliderhit && colliderhit.tag == "Butcher_Bot")
                {
                    butcher_ai = colliderhit.GetComponent<Butcher_AI>();
                    butcher_ai.transform.position += butcher_ai.transform.position.y > transform.position.y ? new Vector3(0, 1, 0) : new Vector3(0, -1, 0);
                    butcher_ai.When_Stun();
                    break;
                }
            }
            return;
        }
        if (Current_State == Board_Style.Down)
        {
            ai.Landing = ai.transform.position.y < transform.position.y ? Up_Location : Down_Loction;
            ai.capsuleCollider2D.enabled = false;
            ai.CanControl = false;
            ai.Move_Speed = 0;
            ai.Crossing = true;
            ai.rb.velocity = Vector2.zero;
            ai.rb.simulated = false;
            ai.pathPointList = null;
            ai.currentIndex = 0;
            ai.transform.DOMove(ai.Landing, 0.5f).OnComplete(() =>
            {
                ai.Crossing = false;
                ai.CanControl = true;
                ai.rb.simulated = true;
                ai.rb.velocity = Vector2.zero;
                ai.capsuleCollider2D.enabled = true;
                ai.Move_Speed = ai.human_data.Speed;
                ai.rb.mass = 1;
                Ban = true;
                Frezzing_Count = Frezzing_Duration;
            });
            return;
        }
    }
    public void Interact_Board_Butcher_Real(Butcher b)
    {
        if (pv != null && !b.photonView.IsMine)
        {
            return;
        }
        b.board = this;
        if (Current_State == Board_Style.Down)
        {
            b.CanControl_B = false;
            b.Speed_B = 0;
            b.breaking = true;
            b.interact_List_B.Interact_Range_B.Remove(b.interact_target);
        }
    }
    public void Interact_Board_Butcher_AI(Butcher_AI ai)
    {
        ai.board = this;
        if (Current_State == Board_Style.Down)
        {
            ai.CanControl_B = false;
            ai.Speed_B = 0;
            ai.breaking = true;
            ai.interact_List_B.Interact_Range_B.Remove(ai.interact_target);
        }
    }
    #endregion
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, Hit_Radius);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(Up_Location, 0.1f);
        Gizmos.DrawSphere(Down_Loction, 0.1f);
    }
}
