using DG.Tweening;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class Player_AI : MonoBehaviour, Istate_Human
{
    public HumanData human_data;
    [Header("移动")]
    public Vector2 inputMove;
    public bool CanControl = true;
    public float Butcher_Ditector;
    [Header("名字")]
    public string Player_name;
    [Header("血量")]
    public bool Player_Alive;
    public bool New_Born;
    [Range(0f, 100f)] public float CurrentHp;
    public float MaxHp;
    public bool IsDown;
    public float Dying_Time;
    public bool Be_Saving;
    public float Saving_Progress;
    public float SelfSaving_Chance;
    [Header("逃命")]
    public Seeker sk;
    [HideInInspector] public List<Vector3> pathPointList;
    public int currentIndex = 0;
    public float pathGenerateInterval = 0.5f;
    public float pathGenerateTimer = 0f;
    public Transform Target_Cipher;
    public List<Cipher_Controller> Cipher_List;
    public float distance;
    public bool Chasing;
    [Header("自动逃跑点位")]
    public List<Transform> escapePoints = new List<Transform>();
    public float checkPointRadius = 3f;
    public float checkPathRadius = 2.5f;
    private int currentEscapePointIndex = 0;
    [Header("受伤")]
    public bool IsHurt;
    public float Hurt_Time;
    public bool Hurt_VFX;
    public bool _hasCountDeath = false;
    [Header("速度")]
    public float Move_Speed;
    public float Hurt_Speed;
    public float Act_SpeedCross;
    public float Act_SpeedDropBoard;
    public float Code_Speed;
    public float Dash_Cooldown;
    public Vector2 Last_Way;
    [Header("交互")]
    Interact_List interact_List;
    public float InteractRadius;
    public Vector2 Coding_position;
    public Vector3 Landing;
    public bool Player_Coding;
    //public bool Saving;
    public bool Crossing;
    [Header("技能")]
    public float Dash_Force;
    public bool Is_Dashing;
    public float Dash_Duration;
    public float Dash_CD;
    [Header("引用")]
    public LayerMask Butcher_Layer;
    public Slider Saving_Slider;
    public TextMeshProUGUI Dying_ForHelp_UI;
    public Rigidbody2D rb;
    public Animator am;
    public CapsuleCollider2D capsuleCollider2D;
    public SpriteRenderer sr;
    public Cipher_Controller ciph;
    public Board board;
    public Window window;
    public Gate_Controller gate;
    public Exit_Manager exit;
    [Header("Fsm")]
    public Istate Current_State;
    public Dictionary<Player_State_Type, Istate> state = new Dictionary<Player_State_Type, Istate>();
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        am = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        sk = GetComponent<Seeker>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        interact_List = GetComponentInChildren<Interact_List>();

        escapePoints.Clear();
        GameObject[] points = GameObject.FindGameObjectsWithTag("Escape_Point");
        foreach (var p in points)
        {
            escapePoints.Add(p.transform);
        }

        name = human_data.top_name;
        Butcher_Ditector = human_data.Heart_Beat_radius;
        Move_Speed = human_data.Speed;
        Act_SpeedCross = human_data.ActSpeed_Cross;
        Act_SpeedDropBoard = human_data.ActSpeed_DropBoard;
        InteractRadius = human_data.Interact_Radius / 2;
        Dash_Force = human_data.Dashing_Force;
        Code_Speed = human_data.CodeSpeed;
        Hurt_Speed = human_data.Speed_Up_When_Hurt;
        Hurt_Time = human_data.Speed_Up_Time;
        SelfSaving_Chance = human_data.Saving_Chance;
        Dash_Cooldown = human_data.Dash_Cool;
        Dash_Duration = human_data.Dash_Duration;
        Dash_CD = human_data.Dash_CD;
        Player_Alive = true;
        MaxHp = human_data.MaxHp;
        Dying_Time = human_data.Aid_Time;
        CurrentHp = MaxHp;
        Dash_Cooldown = 0;
        Chasing = false;
        Saving_Progress = Saving_Slider.value;

        state.Add(Player_State_Type.idle, new Human_Idle_State(this));
        state.Add(Player_State_Type.walk, new Human_Walk_State(this));
        state.Add(Player_State_Type.coding, new Human_Coding_State(this));
        state.Add(Player_State_Type.dash, new Human_Dash_State(this));
        state.Add(Player_State_Type.born, new Human_Born_State(this));
        state.Add(Player_State_Type.needhelp, new Human_Needhelp_State(this));
        state.Add(Player_State_Type.down, new Human_Down_State(this));
        Transition_State(Player_State_Type.idle);
        Last_Cipher_Scan();
        Init_Gate_And_Exit();
    }
    public void Transition_State(Player_State_Type type)
    {
        if (Current_State != null)
        {
            Current_State.OnExit();
        }
        Current_State = state[type];
        Current_State.OnEnter();
    }
    public void OnEnable()
    {
        Gate_Controller.On_Gate_finish += On_Gate_Finished;
    }
    public void OnDisable()
    {
        Gate_Controller.On_Gate_finish -= On_Gate_Finished;
    }
    public void Update()
    {
        if (Current_State != null)
        {
            Current_State.OnUpdate();
        }
        Player_HUD_Manager.instance.Update_Player_HP(this.gameObject, CurrentHp, MaxHp);
        Butcher_Scan();
        Last_Cipher_Scan();
        if (Cipher_List.Count <= 0 )
        {
            Target_Cipher = gate.Finish ? exit.transform : gate.transform;
        }
        if (!Chasing && Cipher_List.Count > 0 && Target_Cipher == null)
        {
            GetCipherTransform(0);
        }
        AutoPath();
        CurrentHp = CurrentHp > MaxHp ? MaxHp : CurrentHp;
        Dash_Cooldown = (Dash_Cooldown > 0) ? Dash_Cooldown - Time.deltaTime : 0;
        if (IsHurt)
        {
            IsHurt = false;
        }
        if (CurrentHp <= 0 && !IsDown)
        {
            Down();
            return;
        }
        if (IsDown)
        {
            Wait_For_Saving();
        }
        else
        {
            Dying_ForHelp_UI.gameObject.SetActive(false);
        }
    }
    public void FixedUpdate()
    {
        if (Current_State != null)
        {
            Current_State.OnFixedUpdate();
        }
        rb.mass = CanControl ? 1 : 99999;
        Last_Way = inputMove.normalized;
    }
    #region 移动
    public void Move()
    {
        if (Crossing)
        {
            return;
        }
        if (CanControl && !Is_Dashing || Chasing)
        {
            Player_Coding = false;
            //if (gate)
            //{
            //    Move_Speed = human_data.Speed;
            //    CanControl = true;
            //    gate.Stop();
            //}
            if (pathPointList == null || currentIndex >= pathPointList.Count)
            {
                inputMove = Vector2.zero;
                rb.velocity = Vector2.zero;
                return;
            }
            inputMove = (pathPointList[currentIndex] - transform.position).normalized;
            rb.velocity = inputMove * Move_Speed;
            if (Vector2.Distance(transform.position, pathPointList[currentIndex]) <= 0.2f)
            {
                currentIndex++;
            }
            if (inputMove.x > 0)
            {
                sr.flipX = false;
            }
            else if (inputMove.x < 0)
            {
                sr.flipX = true;
            }
        }
    }
    #endregion
    #region 自动寻路
    public void AutoPath()
    {
        if (Player_Coding)
        {
            pathPointList = null;
            return;
        }
        pathGenerateTimer += Time.deltaTime;
        if (Chasing)
        {
            if (pathGenerateTimer < pathGenerateInterval)
            {
                return;
            }
            Transform safePoint = GetSafeEscapePoint();
            if (safePoint != null)
            {
                GeneratePath(safePoint.position);
            }
            if (pathPointList != null && currentIndex >= pathPointList.Count)
            {
                currentEscapePointIndex = (currentEscapePointIndex + 1) % escapePoints.Count;
                currentIndex = 0;
            }
            pathGenerateTimer = 0;
            return;
        }
        if (pathGenerateTimer >= pathGenerateInterval)
        {
            if (Target_Cipher != null)
            {
                GeneratePath(Target_Cipher.position);
            }
            pathGenerateTimer = 0;
        }
        //if (!Chasing && Cipher_List.Count > 0)
        //{
        //    if (pathGenerateTimer < pathGenerateInterval)
        //        return;

        //    if (Target_Cipher != null)
        //    {
        //        GeneratePath(Target_Cipher.position);
        //    }

        //    if (pathPointList != null && currentIndex >= pathPointList.Count)
        //    {
        //        pathPointList = null;
        //        currentIndex = 0;
        //    }
        //    pathGenerateTimer = 0;
        //}
    }
    private Transform GetSafeEscapePoint()
    {
        if (escapePoints == null || escapePoints.Count == 0)
        {
            return null;
        }
        for (int i = 0; i < escapePoints.Count; i++)
        {
            int idx = currentEscapePointIndex % escapePoints.Count;
            Transform t = escapePoints[idx];
            if (t == null)
            {
                currentEscapePointIndex++;
                continue;
            }
            bool pointSafe = IsPointSafe(t.position);
            bool pathSafe = IsPathSafe(transform.position, t.position);
            if (pointSafe && pathSafe)
            {
                return t;
            }
            currentEscapePointIndex++;
        }
        return escapePoints[0];
    }

    private bool IsPointSafe(Vector3 pos)
    {
        return Physics2D.OverlapCircleAll(pos, checkPointRadius, Butcher_Layer).Length == 0;
    }

    private bool IsPathSafe(Vector3 start, Vector3 end)
    {
        Path path = sk.StartPath(start, end);
        path.BlockUntilCalculated();
        if (path.vectorPath == null)
        {
            return true;
        }
        for (int i = 0; i < path.vectorPath.Count - 1; i++)
        {
            Vector2 a = path.vectorPath[i];
            Vector2 b = path.vectorPath[i + 1];
            RaycastHit2D hit = Physics2D.Linecast(a, b, Butcher_Layer);
            if (hit.collider != null)
            {
                return false;
            }
        }
        return true;
    }
    public void GeneratePath(Vector3 target)
    {
        currentIndex = 0;
        sk.StartPath(transform.position, target, Path =>
        {
            pathPointList = Path.vectorPath;
        });
    }
    public void GetCipherTransform(int index)
    {
        if (Cipher_List.Count > 0)
        {
            Target_Cipher = Cipher_List[index].transform;
        }
        else
        {
            Target_Cipher = null;
        }
    }
    #endregion

    #region 交互
    public void Interact()
    {
        if (!CanControl || Player_Coding)
        {
            return;
        }
        //if (gameObject.tag == "Player_NeedSave")
        //{
        //    if (SelfSaving_Chance > 0 && !Chasing)
        //    {
        //        Be_Saving = true;
        //    }
        //}
        if (!interact_List || interact_List.Interact_Range.Count <= 0)
        {
            return;
        }
        GameObject interact_target = interact_List.Interact_Range.OrderBy(obj => Vector2.Distance(transform
            .position, obj.transform.position)).FirstOrDefault();
        if (interact_target == null)
        {
            return;
        }
        if (interact_target.TryGetComponent(out Cipher_Controller cipher))
        {
            if (Chasing || cipher.Done)
            {
                return;
            }
            cipher.Interact_Cipher_Player_AI(this);
            //if (cipher.Done || cipher.Coding_Guys.Contains(this))
            //{
            //    return;
            //}
            //ciph = cipher;
            //CanControl = false;
            //Vector2 Cipher_position = interact_target.transform.position;
            //Vector2 OfCipher = (transform.position - interact_target.transform.position).normalized;
            //transform.position = Cipher_position + OfCipher * cipher.Cipher_away_radius * 1.3f;
            //Coding_Enter();
            //cipher.is_coding = true;
            //if (cipher.transform.position.x >= transform.position.x)
            //{
            //    sr.flipX = false;
            //}
            //else
            //{
            //    sr.flipX = true;
            //}
        }
        else if (interact_target.TryGetComponent(out Window window))
        {
            window.Interact_Window_Player_AI(this);
            //window = interact_target.GetComponent<Window>();
            //if (window.Ban)
            //{
            //    return;
            //}
            //Landing = transform.position.y < window.transform.position.y ? window.Up_Location : window.Down_Loction;
            //capsuleCollider2D.enabled = false;
            //CanControl = false;
            //Move_Speed = 0;
            //rb.mass = 9999;
            //Crossing = true;
            //transform.DOMove(Landing, 0.5f).OnComplete(() =>
            //{
            //    Crossing = false;
            //    CanControl = true;
            //    rb.velocity = Vector2.zero;
            //    capsuleCollider2D.enabled = true;
            //    Move_Speed = human_data.Speed;
            //    rb.mass = 1;
            //});
            //window.Ban = true;
            //window.Frezzing_Count = window.Frezzing_Duration;
            //window = null;
            //return;
        }
        else if (interact_target.TryGetComponent(out Board board))
        {
            board.Interact_Board_Player_AI(this);
            //if (board.Ban)
            //{
            //    return;
            //}
            //Butcher butcher;
            //Butcher_AI butcher_ai;
            //if (board.Current_State == Board_Style.Normal)
            //{
            //    board.Change_State(Board_Style.Down);
            //    StartCoroutine(nameof(Drop_Frezze));
            //    Collider2D[] Collider_Hit = Physics2D.OverlapCircleAll(board.transform.position, board.Hit_Radius);
            //    foreach (var colliderhit in Collider_Hit)
            //    {
            //        if (colliderhit.gameObject == this.gameObject)
            //        {
            //            continue;
            //        }
            //        if (colliderhit && colliderhit.tag == "Butcher")
            //        {
            //            butcher = colliderhit.GetComponent<Butcher>();
            //            butcher.transform.position += butcher.transform.position.y > board.transform.position.y ? new Vector3(0, 1, 0) : new Vector3(0, -1, 0);
            //            butcher.When_Stun();
            //            break;
            //        }
            //        if (colliderhit && colliderhit.tag == "Butcher_Bot")
            //        {
            //            butcher_ai = colliderhit.GetComponent<Butcher_AI>();
            //            butcher_ai.transform.position += butcher_ai.transform.position.y > board.transform.position.y ? new Vector3(0, 1, 0) : new Vector3(0, -1, 0);
            //            butcher_ai.When_Stun();
            //            break;
            //        }
            //    }
            //    return;
            //}
            //if (board.Current_State == Board_Style.Down)
            //{
            //    Landing = transform.position.y < board.transform.position.y ? board.Up_Location : board.Down_Loction;
            //    capsuleCollider2D.enabled = false;
            //    CanControl = false;
            //    Move_Speed = 0;
            //    Crossing = true;
            //    rb.velocity = Vector2.zero;
            //    rb.simulated = false;
            //    pathPointList = null;
            //    currentIndex = 0;
            //    transform.DOMove(Landing, 0.5f).OnComplete(() =>
            //    {
            //        Crossing = false;
            //        CanControl = true;
            //        rb.simulated = true;
            //        rb.velocity = Vector2.zero;
            //        capsuleCollider2D.enabled = true;
            //        Move_Speed = human_data.Speed;
            //        rb.mass = 1;
            //        board.Ban = true;
            //        board.Frezzing_Count = board.Frezzing_Duration;
            //    });
            //    return;
            //}
        }
        else if (interact_target.TryGetComponent(out Gate_Controller gate))
        {
            gate.Interact_Gate_Player_AI(this);
            //if (Player_Coding)
            //{
            //    return;
            //}
            //Player_Coding = true;
            //Move_Speed = 0;
            //CanControl = false;
            //rb.velocity = Vector2.zero;
            //gate.Open();
            //return;
        }
    }
    public IEnumerator Drop_Frezze()
    {
        CanControl = false;
        Move_Speed = 0;
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.2f);
        Move_Speed = human_data.Speed;
        CanControl = true;
    }
    public void Init_Gate_And_Exit()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 10000);
        foreach (Collider2D col in cols)
        {
            if (col.gameObject.CompareTag("Gate"))
            {
                gate = col.GetComponent<Gate_Controller>();
                break;
            }
            if (col.gameObject.CompareTag("Exit"))
            {
                exit = col.GetComponent<Exit_Manager>();
            }
        }
    }
    #endregion
    #region 破译
    public void Coding_Check()
    {
        if (ciph && ciph.Is_Calibration && ciph.Calibration_Slider.gameObject.activeSelf && rb.velocity != Vector2.zero)
        {
            ciph.Punish();
            ciph.Calibration_Sign.gameObject.SetActive(false);
            ciph.Calibration_Slider.gameObject.SetActive(false);
            ciph.Is_Calibration = false;
        }
        //if (ciph && inputMove != Vector2.zero)
        //{
        //    Debug.Log("tuoli");
        //    Coding_Exit();
        //    ciph.is_coding = false;
        //    //am.SetBool("IsCoding", false);
        //    //人物回到移动动画，顺便关闭修机ui和暂停修机
        //    ciph = null;
        //}
    }
    public void Coding_Enter()
    {
        if (ciph && !Player_Coding && !ciph.Done)
        {
            Player_Coding = true;
            rb.velocity = Vector2.zero;
            am.SetBool("IsCoding", true);
            //ciph.Coding_members++;
            ciph.Coding_Guys.Add(this);
        }
    }
    public void Coding_Exit()
    {
        if (ciph != null)
        {
            ciph.Coding_Guys.Remove(this);
            ciph.is_coding = false;
        }
        Player_Coding = false;
        CanControl = true;
        am.SetBool("IsCoding", false);
        ciph = null;
        FindNextCipher();
    }
    public void Last_Cipher_Scan()
    {
        Cipher_List.Clear();
        Collider2D[] chaseColliders = Physics2D.OverlapCircleAll(transform.position, 1000);
        foreach (Collider2D collider in chaseColliders)
        {
            if (collider.CompareTag("Cipher"))
            {
                Cipher_Controller cipher_ai = collider.GetComponent<Cipher_Controller>();
                if (cipher_ai.Done)
                {
                    continue;
                }
                Cipher_List.Add(cipher_ai);
            }
        }
        Cipher_List.Sort((a, b) => Vector2.Distance(transform.position, a.transform.position).CompareTo(Vector2.Distance(transform.position, b.transform.position)));
    }
    public void On_Gate_Finished()
    {
        if (!Player_Coding)
        {
            return;
        }
        transform.Translate(-0.75f, 0.5f, 0);
        Player_Coding = false;
        CanControl = true;
        rb.velocity = Vector2.zero;

    }
    public void FindNextCipher()
    {
        Last_Cipher_Scan(); 
        if (Cipher_List.Count > 0)
        {
            GetCipherTransform(0);
        }
        else
        {
            Target_Cipher = null;
        }
    }
    #endregion
    #region 技能
    public void Skill()
    {
        if (rb.velocity == Vector2.zero || Dash_Cooldown > 0)
        {
            return;
        }
        Is_Dashing = true;
        am.SetBool("IsDashing", true);
        Debug.Log(Last_Way * 100f);
        rb.velocity = Last_Way * Dash_Force;
        rb.AddForce(rb.velocity, ForceMode2D.Impulse);
        Invoke(nameof(Dashing_Time), Dash_Duration);
        //transform.position += new Vector3(2, 0, 0);
    }
    public void Dashing_Time()
    {
        Is_Dashing = false;
        am.SetBool("IsDashing", false);
        Dash_Cooldown = Dash_CD;
    }
    #endregion
    #region 受伤和倒下
    public void GetHurt(float damage)
    {
        if (Is_Dashing)
        {
            return;
        }
        CurrentHp -= damage;
        Sound_Manager.instance.Play_sfx(Sound_Manager.instance.Hurt_Sound);
        Hurt_VFX = true;
        if (CurrentHp > 0)
        {
            StartCoroutine(SpeedUp_When_Hurt());
        }
        else
        {
            Down();
        }
    }
    public void Down()
    {
        Player_Alive = false;
        capsuleCollider2D.enabled = false;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        IsDown = true;
        sr.flipX = false;
        CanControl = false;
        Coding_Exit();
        if (ciph)
        {
            Coding_Exit();
            ciph.is_coding = false;
            ciph = null;
        }
        gameObject.tag = "Player_NeedSave";
        gameObject.layer = LayerMask.NameToLayer("Player_Down");
        am.SetBool("IsDown", true);
    }
    public IEnumerator SpeedUp_When_Hurt()
    {
        CanControl = true;
        Move_Speed *= Hurt_Speed;
        sr.color = Color.red;
        yield return new WaitForSeconds(Hurt_Time);
        Move_Speed = human_data.Speed;
        sr.color = Color.white;
    }
    public void Wait_For_Saving()
    {
        bool isSafe = !Chasing && Physics2D.OverlapCircleAll(transform.position, 12f, Butcher_Layer).Length == 0;
        if (isSafe && SelfSaving_Chance > 0 && !Be_Saving)
        {
            Be_Saving = true;
        }
        Dying_ForHelp_UI.gameObject.SetActive(true);
        if (!Be_Saving)
        {
            if (SelfSaving_Chance == 0)
            {
                Body_Disappear();
                Dead();
            }
            Dying_ForHelp_UI.color = Color.red;
            Dying_ForHelp_UI.text = $"You're Down! You \nhave {SelfSaving_Chance} chance to \nrevive:{(int)Dying_Time}";
            Dying_Time = Dying_Time > 0 ? Dying_Time - Time.deltaTime : 0;
        }
        else
        {
            Dying_ForHelp_UI.color = Color.blue;
            Dying_ForHelp_UI.text = $"Hold on!\nSelf Saving......";
            Saving_Slider.gameObject.SetActive(true);
            Saving_Slider.value += 0.5f * Time.deltaTime / Saving_Slider.maxValue;
        }
    }
    public void Saving_Check()
    {
        if (gameObject && Be_Saving && rb.velocity != Vector2.zero)
        {
            Be_Saving = false;
        }
    }
    public void Get_Aid()
    {
        if (Saving_Slider.value == Saving_Slider.maxValue)
        {
            capsuleCollider2D.enabled = true;
            SelfSaving_Chance--;
            Saving_Slider.value = 0;
            CanControl = true;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            IsDown = false;
            CurrentHp = MaxHp / 2;
            Be_Saving = false;
            gameObject.tag = "Player";
            inputMove = Vector2.zero;
            Transition_State(Player_State_Type.born);
            if (ciph != null)
            {
                ciph = null;
            }
            am.SetBool("IsBorn", true);
            am.SetBool("IsDown", false);
        }
        Saving_Progress = Saving_Slider.value;
        Saving_Slider.gameObject.SetActive((IsDown && Be_Saving) ? true : false);
        if (Dying_Time <= 0)
        {
            Body_Disappear();
        }
    }
    public void Body_Disappear()
    {
        Color dead_color = sr.color;
        dead_color.a = Mathf.Lerp(sr.color.a, 0, Time.deltaTime * 3f);
        Dying_ForHelp_UI.text = $"";
        sr.color = dead_color;
        if (sr.color.a <= 0.02f && !_hasCountDeath)
        {
            _hasCountDeath = true;
            Victory_Manager.instance.Dead();
            Destroy(gameObject);
        }
    }
    public void Dead()
    {
        if (sr.color.a <= 0.01)
        {
            Destroy(gameObject);
        }
    }
    #endregion
    #region 新生
    public void Born_Check()
    {
        Debug.Log("有新生");
        New_Born = false;
        am.SetBool("IsBorn", true);
    }
    public void Wait_For_Born()
    {
        am.SetBool("IsBorn", false);
        CanControl = true;
        Player_Alive = true;
        gameObject.tag = "Player";
        gameObject.layer = LayerMask.NameToLayer("Player");
    }
    #endregion
    public void Butcher_Scan()
    {
        Collider2D[] Heart_Beat = Physics2D.OverlapCircleAll(transform.position, Butcher_Ditector, Butcher_Layer);
        if (Heart_Beat.Length > 0)
        {
            if (Chasing)
            {
                return;
            }
            Chasing = true;
            if (!IsDown)
            {
                CanControl = true;
                Coding_Exit();
                Target_Cipher = null;
                Transition_State(Player_State_Type.walk);
                pathGenerateTimer = pathGenerateInterval;
                AutoPath();
            }
        }
        else
        {
            Chasing = false;
        }
    }
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, InteractRadius);
    }
}
