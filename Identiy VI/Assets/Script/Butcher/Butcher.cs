using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Butcher : MonoBehaviour, Istate_Butcher
{
    public ButcherData butcher_data;
    [Header("ÒÆ¶¯")]
    public InputActions act_B;
    public Vector2 inputMove_B;
    public ButcherInput butcherInput;
    public bool CanControl_B = true;
    [Header("Ãû×Ö")]
    public string Butcher_name;
    [Header("½»»¥")]
    public GameObject interact_target;
    public float Interact_Radius_B;
    public Vector3 Landing_B;
    public bool Crossing_B;
    public bool breaking;
    [Header("ÉËº¦")]
    public Vector2 lastMoveDir = Vector2.right;
    public float Damage_B;
    public float Attack_Distance_B;
    public float Attack_Stun_Time_B;
    public bool Is_Attack_B;
    [Header("Ñ£ÔÎ")]
    public float StunTime;
    public bool Is_Stun_B;
    [Header("ËÙ¶È")]
    public float Speed_B;
    public float Act_SpeedCross_B;
    public float Act_SpeedBreakBoard_B;
    [Header("ÒýÓÃ")]
    public LayerMask Player_Layer;
    public LineRenderer lr;
    public Rigidbody2D rb;
    public Animator am;
    public SpriteRenderer sr;
    public CapsuleCollider2D col;
    public Collider2D collider_Human;
    public Board board;
    public Window window;
    public Image Interact_Button;
    public TextMeshProUGUI Interact_Key_Name;
    public Interact_List_B interact_List_B;
    public GameObject Option;
    public GameObject After_Die;
    [Header("Fsm")]
    public Istate Current_State_B;
    public Dictionary<Butcher_State_Type, Istate> state_B = new Dictionary<Butcher_State_Type, Istate>();
    private void Awake()
    {
        act_B = new InputActions();
        rb = GetComponent<Rigidbody2D>();
        am = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<CapsuleCollider2D>();
        lr = GetComponent<LineRenderer>();
        interact_List_B = GetComponentInChildren<Interact_List_B>();
        butcherInput.EnabaleButcherInput();
        act_B.Enable();
        name = butcher_data.top_name;
        Interact_Radius_B = butcher_data.Interact_Radius;
        Damage_B = butcher_data.Damage;
        Speed_B = butcher_data.Speed;
        Act_SpeedCross_B = butcher_data.ActSpeed_Cross;
        Act_SpeedBreakBoard_B = butcher_data.ActSpeed_BreakBoard;
        StunTime = butcher_data.StunTime;
        Attack_Distance_B = butcher_data.Attack_Distance;
        Attack_Stun_Time_B = butcher_data.Attack_Stun_Time;

        After_Die = GameObject.FindGameObjectWithTag("After_Die");
        After_Die.SetActive(false);

        state_B.Add(Butcher_State_Type.idle, new Butcher_Idle_State(this));
        state_B.Add(Butcher_State_Type.walk, new Butcher_Walk_State(this));
        state_B.Add(Butcher_State_Type.attack, new Butcher_Attack_State(this));
        state_B.Add(Butcher_State_Type.breaking, new Butcher_Break_State(this));
        state_B.Add(Butcher_State_Type.show, new Butcher_Show_State(this));
        state_B.Add(Butcher_State_Type.notshow, new Butcher_NotShow_State(this));
        state_B.Add(Butcher_State_Type.stun, new Butcher_Stun_State(this));
        Transition_State_B(Butcher_State_Type.idle);
    }
    public void Transition_State_B(Butcher_State_Type type)
    {
        if (Current_State_B != null)
        {
            Current_State_B.OnExit();
        }
        Current_State_B = state_B[type];
        Current_State_B.OnEnter();
    }
    public void OnEnable()
    {
        butcherInput.onMove += Move;
        butcherInput.onInteract += Interact;
        butcherInput.onAttack += Attack;
        butcherInput.onEscape += Escape;
    }
    private void OnDisable()
    {
        butcherInput.onMove -= Move;
        butcherInput.onInteract -= Interact;
        butcherInput.onAttack -= Attack;
        butcherInput.onEscape -= Escape;
    }
    public void Update()
    {
        Current_State_B.OnUpdate();
        interact_target = interact_List_B.Interact_Range_B.OrderBy(obj => Vector2.Distance(transform
            .position, obj.transform.position)).FirstOrDefault();
        Interact_Button.gameObject.SetActive(interact_List_B.Interact_Range_B.Count > 0 && !Crossing_B);
        if (interact_target)
        {
            Interact_Button.transform.position = interact_target.transform.position;
        }
        Interact_Key_Name.text = $"Space";
        if (interact_List_B.Interact_Range_B.Count > 0)
        {
            Interact_Check_B();
        }
    }
    public void FixedUpdate()
    {
        Current_State_B.OnFixedUpdate();
        rb.mass = CanControl_B ? 1 : 99999;
    }
    #region ÒÆ¶¯
    public void Move()
    {
        Move(inputMove_B);
    }
    public void Move(Vector2 moveInput)
    {
        if (moveInput.magnitude > 0.01f)
        {
            lastMoveDir = moveInput.normalized;
        }
        inputMove_B = moveInput;
        if (CanControl_B)
        {
            Cursor.visible = false;
            if (moveInput.x > 0)
            {
                sr.flipX = false;
            }
            else if (moveInput.x < 0)
            {
                sr.flipX = true;
            }
        }
    }
    #endregion
    #region ½»»¥
    public void Interact()
    {
        if (!CanControl_B)
        {
            return;
        }
        if (!interact_List_B || interact_List_B.Interact_Range_B.Count <= 0)
        {
            return;
        }
        if (interact_target == null)
        {
            return;
        }
        if (interact_target.TryGetComponent(out Board board))
        {
            board.Interact_Board_Butcher_Real(this);
            Sound_Manager.instance.Play_sfx(Sound_Manager.instance.Breaking_Sound);
            Sound_Manager.instance.Play_sfx(Sound_Manager.instance.Breaking_Sound);
            Sound_Manager.instance.Play_sfx(Sound_Manager.instance.Breaking_Sound);
        }
        else if (interact_target.TryGetComponent(out Window window))
        {
            window.Interact_Window_Butcher_Real(this);
            Sound_Manager.instance.Play_sfx(Sound_Manager.instance.Crossing_Sound);
        }
    }
    public void Interact_Check_B()
    {
        if (interact_target.TryGetComponent(out Board b))
        {
            if (b.Current_State == Board_Style.Normal || b.Current_State == Board_Style.Broken)
            {
                interact_List_B.Interact_Range_B.Remove(interact_target);
            }
        }
    }
    #endregion
    #region Ëé°å
    public void Break_The_Board()
    {
        if (board)
        {
            board.Change_State(Board_Style.Broken);
            breaking = false;
        }
    }
    public void Broken_Check()
    {
        Speed_B = butcher_data.Speed;
        CanControl_B = true;
    }
    #endregion
    #region ¹¥»÷
    public void Attack()
    {
        am.SetTrigger("IsAttack");
        Is_Attack_B = true;
    }
    public void Butcher_Attack()
    {
        if (!CanControl_B)
        {
            return;
        }
        Sound_Manager.instance.Play_sfx(Sound_Manager.instance.Attack_Sound);
        Vector2 dir = inputMove_B.magnitude > 0.01f ? inputMove_B.normalized : lastMoveDir;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Vector2 center = (Vector2)transform.position + dir * (Attack_Distance_B * 0.5f);
        Vector2 size = new Vector2(Attack_Distance_B, col.size.y);
        collider_Human = Physics2D.OverlapBox(center, size, angle, Player_Layer);
        if (collider_Human)
        {
            if (collider_Human.TryGetComponent(out Player player_r))
            {
                player_r.GetHurt(Damage_B);
                player_r.IsHurt = true;
                player_r.rb.velocity = Vector2.zero;
                player_r = null;
            }
            else if (collider_Human.TryGetComponent(out Player_AI player_a))
            {
                player_a.GetHurt(Damage_B);
                player_a.IsHurt = true;
                player_a.rb.velocity = Vector2.zero;
                player_a = null;
            }
            else
            {
                collider_Human = null;
            }
        }
        Is_Attack_B = false;
    }
    public void Attack_Recovery()
    {
        StartCoroutine(Attack_Recovery_Enumerator());
    }
    public IEnumerator Attack_Recovery_Enumerator()
    {
        Speed_B = 0;
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.5f);
        CanControl_B = true;
        Speed_B = butcher_data.Speed;
    }
    #endregion
    #region ²Áµ¶
    //public void Show_Knife()
    //{
    //    if (collider_Human)
    //    {
    //        am.Play("Butcher_show");
    //        StartCoroutine(Knife());
    //    }
    //    else
    //    {
    //        am.Play("Butcher_notshow");
    //    }
    //    collider_Human = null;
    //    //Is_Attack_B = false;
    //}
    //public IEnumerator Knife()
    //{
    //    Speed_B /= 2;
    //    yield return new WaitForSeconds(0.5f);
    //    Speed_B = butcher_data.Speed;
    //}
    #endregion
    #region Ñ£ÔÎ
    public void When_Stun()
    {
        Is_Stun_B = true;
    }
    #endregion
    public void Escape()
    {
        Option.gameObject.SetActive(!Option.gameObject.activeSelf);
        Cursor.visible = Option.gameObject.activeSelf ? true : false;
    }
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 dir = inputMove_B.magnitude > 0.01f ? inputMove_B.normalized : (sr.flipX ? Vector2.left : Vector2.right);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Vector2 center = (Vector2)transform.position + dir * (Attack_Distance_B * 0.5f);
        Vector2 size = new Vector2(Attack_Distance_B, col.size.y / 2.5f);
        Gizmos.matrix = Matrix4x4.TRS(center, Quaternion.Euler(0, 0, angle), Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, size);
    }
}
