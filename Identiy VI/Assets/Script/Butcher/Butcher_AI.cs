using DG.Tweening;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Butcher_AI : MonoBehaviour, Istate_Butcher
{
    public ButcherData butcher_data;
    [Header("ÒÆ¶¯")]
    public Vector2 inputMove_B;
    public bool CanControl_B = true;
    [Header("Ãû×Ö")]
    public string Butcher_name;
    [Header("×·»÷")]
    public Seeker sk;
    [HideInInspector] public List<Vector3> pathPointList;
    public int currentIndex = 0;
    public float pathGenerateInterval = 0.5f;
    public float pathGenerateTimer = 0f;
    public Transform Target_Human;
    public float distance;
    [Header("½»»¥")]
    public Interact_List_B interact_List_B;
    public GameObject interact_target;
    public float Interact_Radius_B;
    public Vector3 Landing_B;
    public bool Crossing_B;
    public bool breaking;

    public string cur_scene;
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
    public LayerMask Wall_Layer;
    public Rigidbody2D rb;
    public Animator am;
    public SpriteRenderer sr;
    public CapsuleCollider2D col;
    public Collider2D collider_Human;
    public Board board;
    public Window window;
    [Header("Fsm")]
    public Istate Current_State_B;
    public Dictionary<Butcher_State_Type, Istate> state_B = new Dictionary<Butcher_State_Type, Istate>();
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        am = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<CapsuleCollider2D>();
        sk = GetComponent<Seeker>();
        interact_List_B = GetComponentInChildren<Interact_List_B>();
        name = butcher_data.top_name;
        Interact_Radius_B = butcher_data.Interact_Radius;
        Damage_B = butcher_data.Damage;
        Speed_B = butcher_data.Speed / 1.3f;
        Act_SpeedCross_B = butcher_data.ActSpeed_Cross;
        Act_SpeedBreakBoard_B = butcher_data.ActSpeed_BreakBoard;
        StunTime = butcher_data.StunTime;
        Attack_Distance_B = butcher_data.Attack_Distance;
        Attack_Stun_Time_B = butcher_data.Attack_Stun_Time;

        state_B.Add(Butcher_State_Type.idle, new Butcher_Idle_State(this));
        state_B.Add(Butcher_State_Type.walk, new Butcher_Walk_State(this));
        state_B.Add(Butcher_State_Type.attack, new Butcher_Attack_State(this));
        state_B.Add(Butcher_State_Type.breaking, new Butcher_Break_State(this));
        state_B.Add(Butcher_State_Type.show, new Butcher_Show_State(this));
        state_B.Add(Butcher_State_Type.notshow, new Butcher_NotShow_State(this));
        state_B.Add(Butcher_State_Type.stun, new Butcher_Stun_State(this));
        Transition_State_B(Butcher_State_Type.idle);
        Interact();

        cur_scene = SceneManager.GetActiveScene().name;
        if(cur_scene== "Start Hall")
        {
            Damage_B = 0;
        }
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
    public void Update()
    {
        GetPlayerTransform();
        AutoPath();
        interact_target = interact_List_B.Interact_Range_B.OrderBy(obj => Vector2.Distance(transform
            .position, obj.transform.position)).FirstOrDefault();
        Current_State_B.OnUpdate();
    }
    public void FixedUpdate()
    {
        Current_State_B.OnFixedUpdate();
        rb.mass = CanControl_B ? 1 : 99999;
    }
    #region ÒÆ¶¯
    public void Move()
    {
        if (CanControl_B)
        {
            if (pathPointList == null || currentIndex >= pathPointList.Count)
            {
                inputMove_B = Vector2.zero;
                rb.velocity = Vector2.zero;
                return;
            }
            //inputMove_B = moveInput;
            inputMove_B = (pathPointList[currentIndex] - transform.position).normalized;
            rb.velocity = inputMove_B * Speed_B;
            if (inputMove_B.magnitude > 0.01f)
            {
                lastMoveDir = inputMove_B;
            }
            if (Vector2.Distance(transform.position, pathPointList[currentIndex]) <= 0.2f)
            {
                currentIndex++;
            }
            if (inputMove_B.x > 0)
            {
                sr.flipX = false;
            }
            else if (inputMove_B.x < 0)
            {
                sr.flipX = true;
            }
        }
    }
    #endregion
    #region ×Ô¶¯Ñ°Â·
    public void AutoPath()
    {
        if (Target_Human == null)
        {
            return;
        }
        pathGenerateTimer += Time.deltaTime;
        if (pathGenerateTimer >= pathGenerateInterval)
        {
            GeneratePath(Target_Human.position);
            pathGenerateTimer = 0;
        }
        if (pathPointList == null || pathPointList.Count <= 0 || pathPointList.Count <= currentIndex)
        {
            GeneratePath(Target_Human.position);
        }
        else if (currentIndex < pathPointList.Count)
        {
            if (Vector2.Distance(transform.position, pathPointList[currentIndex]) <= 0.1f)
            {
                currentIndex++;
                if (currentIndex >= pathPointList.Count)
                {
                    GeneratePath(Target_Human.position);
                }
            }
        }
    }
    public void GeneratePath(Vector3 target)
    {
        currentIndex = 0;
        sk.StartPath(transform.position, target, Path =>
        {
            pathPointList = Path.vectorPath;
        });
    }
    public void GetPlayerTransform()
    {
        Collider2D[] chaseColliders = Physics2D.OverlapCircleAll(transform.position, 1000, Player_Layer);
        var valid = chaseColliders.Where(o =>
        {
            if (o.TryGetComponent(out Player pl))
            {
                return pl.Player_Alive && !pl.IsDown;
            }
            if (o.TryGetComponent(out Player_AI pai))
            {
                return pai.Player_Alive && !pai.IsDown;
            }
            return false;
        })
        .OrderBy(obj => Vector2.Distance(transform.position, obj.transform.position))
        .FirstOrDefault();
        Target_Human = valid ? valid.transform : null;
        if (Target_Human != null)
        {
            distance = Vector2.Distance(Target_Human.position, transform.position);
        }
        else
        {
            Target_Human = null;
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
            board.Interact_Board_Butcher_AI(this);
            if(cur_scene== "¡°The Red Church¡±")
            {
                Sound_Manager.instance.Play_sfx(Sound_Manager.instance.Breaking_Sound);
                Sound_Manager.instance.Play_sfx(Sound_Manager.instance.Breaking_Sound);
                Sound_Manager.instance.Play_sfx(Sound_Manager.instance.Breaking_Sound);
            }
        }
        else if (interact_target.TryGetComponent(out Window window))
        {
            window.Interact_Window_Butcher_AI(this);
            if(cur_scene== "The Red Church¡±")
            {
                Sound_Manager.instance.Play_sfx(Sound_Manager.instance.Crossing_Sound);
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
        if (cur_scene == "The Red Church¡±")
        {
            Sound_Manager.instance.Play_sfx(Sound_Manager.instance.Attack_Sound);
        }
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
    public void AttackEnd()
    {

    }
    public IEnumerator Attack_Recovery_Enumerator()
    {
        CanControl_B = false;
        Speed_B = 0;
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.5f);
        CanControl_B = true;
        Speed_B = butcher_data.Speed;
    }
    #endregion
    #region ²Áµ¶
    #endregion
    #region Ñ£ÔÎ
    public void When_Stun()
    {
        Is_Stun_B = true;
    }
    #endregion
    //public void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Vector2 dir = inputMove_B.magnitude > 0.01f ? inputMove_B.normalized : (sr.flipX ? Vector2.left : Vector2.right);
    //    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    //    Vector2 center = (Vector2)transform.position + dir * (Attack_Distance_B * 0.5f);
    //    Vector2 size = new Vector2(Attack_Distance_B, col.size.y / 2.5f);
    //    Gizmos.matrix = Matrix4x4.TRS(center, Quaternion.Euler(0, 0, angle), Vector3.one);
    //    Gizmos.DrawWireCube(Vector3.zero, size);
    //}
}