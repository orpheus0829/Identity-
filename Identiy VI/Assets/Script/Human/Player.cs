using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using UnityEngine.SceneManagement;
using Cinemachine;
public class Player :  MonoBehaviour, Istate_Human
{
    public HumanData human_data;
    [Header("移动")]
    public InputActions act;
    public Vector2 inputMove;
    public PlayerInput playerInput;
    public bool CanControl = true;
    public float Butcher_Ditector;
    [Header("名字")]
    public string Player_name;
    [Header("血量")]
    public bool Player_Alive;
    public bool New_Born;
    [Range(0f,100f)]public float CurrentHp;
    public float MaxHp;
    //public float Med_Pack;
    public bool IsDown;
    public float Dying_Time;
    public bool Be_Saving;
    public float Saving_Progress;
    public float SelfSaving_Chance;
    [Header("受伤")]
    public bool IsHurt;
    public bool Hurt_VFX;
    public float Hurt_Time;
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
    public GameObject interact_target;
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
    public Image Interact_Button;
    public TextMeshProUGUI Interact_Key_Name;
    public TextMeshProUGUI Dash_Key_Name;
    public Interact_List interact_List;
    public GameObject Option;
    public GameObject After_Die;
    [Header("Fsm")]
    public Istate Current_State;
    public Dictionary<Player_State_Type, Istate> state = new Dictionary<Player_State_Type, Istate>();
    private void Awake()
    {
        act = new InputActions();
        rb = GetComponent<Rigidbody2D>();
        am = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        interact_List = GetComponentInChildren<Interact_List>();
        playerInput.EnabaleHumanInput();

        After_Die = GameObject.FindGameObjectWithTag("After_Die");
        After_Die.SetActive(false);

        act.Enable();
        name = human_data.top_name;
        Butcher_Ditector = human_data.Heart_Beat_radius;
        Move_Speed = human_data.Speed;
        Act_SpeedCross = human_data.ActSpeed_Cross;
        Act_SpeedDropBoard = human_data.ActSpeed_DropBoard;
        InteractRadius = human_data.Interact_Radius;
        Dash_Force = human_data.Dashing_Force;
        Dash_Duration = human_data.Dash_Duration;
        Dash_CD = human_data.Dash_CD;
        Code_Speed = human_data.CodeSpeed;
        Hurt_Speed = human_data.Speed_Up_When_Hurt;
        Hurt_Time = human_data.Speed_Up_Time;
        SelfSaving_Chance = human_data.Saving_Chance;
        Dash_Cooldown = human_data.Dash_Cool;
        Player_Alive = true;
        MaxHp = human_data.MaxHp;
        Dying_Time = human_data.Aid_Time;
        CurrentHp = MaxHp;
        Dash_Cooldown = 0;
        Saving_Progress = Saving_Slider.value;

        state.Add(Player_State_Type.idle, new Human_Idle_State(this));
        state.Add(Player_State_Type.walk, new Human_Walk_State(this));
        state.Add(Player_State_Type.coding, new Human_Coding_State(this));
        state.Add(Player_State_Type.dash, new Human_Dash_State(this));
        state.Add(Player_State_Type.born, new Human_Born_State(this));
        state.Add(Player_State_Type.needhelp, new Human_Needhelp_State(this));
        state.Add(Player_State_Type.down, new Human_Down_State(this));
        Transition_State(Player_State_Type.idle);
    }
    public void Start()
    {
        Interact();
        Skill();
    }
    public void Transition_State(Player_State_Type type)
    {
        if (Current_State !=null)
        {
            Current_State.OnExit();
        }
        Current_State = state[type];
        Current_State.OnEnter();
    }
    public void OnEnable()
    {
        playerInput.onMove += Move;
        playerInput.onInteract += Interact;
        playerInput.onSkill += Skill;
        playerInput.onEscape += Escape;
        Gate_Controller.On_Gate_finish += On_Gate_Finished;
    }
    private void OnDisable()
    {
        playerInput.onMove -= Move;
        playerInput.onInteract -= Interact;
        playerInput.onSkill -= Skill;
        playerInput.onEscape -= Escape;
        Gate_Controller.On_Gate_finish -= On_Gate_Finished;
    }
    public void Update()
    {
        Current_State.OnUpdate();
        CurrentHp = CurrentHp > MaxHp ? MaxHp : CurrentHp;
        Player_HUD_Manager.instance.Update_Player_HP(this.gameObject, CurrentHp, MaxHp);
        Dash_Cooldown = (Dash_Cooldown > 0) ? Dash_Cooldown - Time.deltaTime : 0;
        if (!Player_Coding)
        {
            interact_target = interact_List.Interact_Range.OrderBy(obj => Vector2.Distance(transform
            .position, obj.transform.position)).FirstOrDefault();
        }
        Interact_Button.gameObject.SetActive(interact_List.Interact_Range.Count > 0 && !Crossing && !Player_Coding);
        Interact_Button.transform.position = interact_List.Interact_Range.Count > 0 ? interact_target.transform.position : new Vector3(100000, 10000, 1);
        Interact_Key_Name.text = $"Space";
        if (interact_List.Interact_Range.Count > 0)
        {
            Interact_Check();
        }
        if (IsHurt)
        {
            IsHurt = false;
        }
        if (CurrentHp <= 0)
        {
            Down();
        }
        if (IsDown)
        {
            Wait_For_Saving();
        }
        else
        {
            Dying_ForHelp_UI.gameObject.SetActive(false);
        }
        Sound_Control_Human();
    }
    public void FixedUpdate()
    {
        Current_State.OnFixedUpdate();
        rb.mass = CanControl ? 1 : 99999;
        Last_Way = inputMove.normalized;
    }
    #region 移动
    public void Move()
    {
        if (CanControl && !Is_Dashing)
        {
            Cursor.visible = false;
            //if (gate && !Player_Coding)
            //{
            //    gate.Stop();
            //    gate = null;
            //}
            //if (gate && rb.velocity != Vector2.zero)
            //{
            //    Move_Speed = human_data.Speed;
            //    gate.Stop();
            //    gate = null;
            //    Player_Coding = false;
            //}
            Move(inputMove);
        }
    }
    public void Move(Vector2 moveInput)
    {
        inputMove = moveInput;
        if (CanControl)
        {
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
    #region 交互
    public void Interact()
    {
        Debug.Log("可以交互");
        if (!CanControl)
        {
            return;
        }
        if (ciph && ciph.Is_Calibration)
        {
            ciph.Calibration_Slider.value += 0.4f;
            return;
        }
        if (gameObject.tag == "Player_NeedSave")
        {
            Debug.Log("xxx");
            if (SelfSaving_Chance > 0)
            {
                Be_Saving = true;
            }
        }
        if (IsDown || !Player_Alive)
        {
            return;
        }
        if (!interact_List || interact_List.Interact_Range.Count <= 0)
        {
            return;
        }
        if (interact_target == null) {
            return;
        }
        if(interact_target.TryGetComponent(out Cipher_Controller cipher))
        {
            cipher.Interact_Cipher_Player_Real(this);
        }
        else if(interact_target.TryGetComponent(out Window window))
        {
            window.Interact_Window_Player_Real(this);
            Sound_Manager.instance.Play_sfx(Sound_Manager.instance.Crossing_Sound);
        }
        else if(interact_target.TryGetComponent(out Board board))
        {
            board.Interact_Board_Player_Real(this);
            Sound_Manager.instance.Play_sfx(Sound_Manager.instance.Crossing_Sound);
        }
        else if(interact_target.TryGetComponent(out Gate_Controller gate))
        {
            this.gate = gate;
            Player_Coding = true;
            gate.Interact_Gate_Player_Real(this);
        }
    }
    public void Interact_Check()
    {
        if (interact_target.TryGetComponent(out Board b) && b.Current_State == Board_Style.Broken)
        {
            interact_List.Interact_Range.Remove(interact_target);
        }
        if (interact_target.TryGetComponent(out Cipher_Controller c) && c.gameObject.tag == "Cipher_CantCoding")
        {
            interact_List.Interact_Range.Remove(interact_target);
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
#endregion
    #region 破译
    public void Coding_Check()
    {
        if (ciph&&ciph.Is_Calibration && ciph.Calibration_Slider.gameObject.activeSelf && rb.velocity != Vector2.zero)
        {
            ciph.Punish();
            ciph.Calibration_Sign.gameObject.SetActive(false);
            ciph.Calibration_Slider.gameObject.SetActive(false);
            ciph.Is_Calibration = false;
        }
        if (ciph && inputMove!=Vector2.zero)
        {
            Coding_Exit();
            ciph.is_coding = false;
            //am.SetBool("IsCoding", false);
            //人物回到移动动画，顺便关闭修机ui和暂停修机
            ciph = null;
        }
    }
    public void Coding_Enter()
    {
        rb.drag = 40000000;
        if (ciph && !Player_Coding)
        {
            Player_Coding = true;
            //ciph.Coding_members++;
            ciph.Coding_Guys.Add(this);
        }
    }
    public void Coding_Exit()
    {
        rb.drag = 0;
        if (ciph && Player_Coding)
        {
            ciph.Has_Real_Player = false;
            Player_Coding = false;
            am.SetBool("IsCoding", false);
            //Mathf.Max(ciph.Coding_members --, 0);
            ciph.Coding_Guys.Remove(this);
        }
    }
    public void On_Gate_Finished()
    {
        if (!Player_Coding)
        {
            return;
        }
        gate = null;
        transform.Translate(-0.55f, 0.5f, 0);
        Player_Coding = false;
        CanControl = true;
        rb.velocity = Vector2.zero;
    }
    #endregion
    #region 技能
    public void Skill()
    {
        if (rb.velocity == Vector2.zero || Dash_Cooldown > 0)
        {
            return;
        }
        Player_UI.instance.Dash_Start();
        Is_Dashing = true;
        am.SetBool("IsDashing", true);
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
        OnDisable();
        playerInput.onInteract += Interact;
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
        Move_Speed *= Hurt_Speed;
        sr.color = Color.red;
        yield return new WaitForSeconds(Hurt_Time);
        Move_Speed = human_data.Speed;
        sr.color = Color.white;
    }
    public void Wait_For_Saving()
    {
        sr.flipX = true;
        Dying_ForHelp_UI.gameObject.SetActive(true);
        if (!Be_Saving)
        {
            if (SelfSaving_Chance == 0)
            {
                Body_Disappear();
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
            //Saving = false;
            Be_Saving = false;
            //injured = null;
        }
    }
    public void Get_Aid()
    {
        if (Saving_Slider.value == Saving_Slider.maxValue)
        {
            capsuleCollider2D.enabled = true;
            SelfSaving_Chance--;
            Saving_Slider.value = 0;
            playerInput.onInteract -= Interact;
            OnEnable();
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
        dead_color.a=Mathf.Lerp(sr.color.a, 0, Time.deltaTime * 3f);
        Dying_ForHelp_UI.text = $"";
        sr.color = dead_color;
        Dead();
    }
    public void Dead()
    {
        if (sr.color.a <= 0.01 && !_hasCountDeath)
        {
            _hasCountDeath = true;
            act.Disable();
            Victory_Manager.instance.Dead();
            Destroy(gameObject);
            Debug.Log("死后观战");
            GameObject vCamera = GameObject.FindGameObjectWithTag("Cinemachine_Camera");
            CinemachineVirtualCamera v = vCamera.GetComponent<CinemachineVirtualCamera>();
            v.transform.position = new Vector3(40, 0, -10);
            v.m_Lens.OrthographicSize = 18;

            int ui= LayerMask.NameToLayer("UI");
            Canvas[] allUI = FindObjectsOfType<Canvas>(true);
            foreach(Canvas cv in allUI)
            {
                if (cv.gameObject.layer == ui)
                {
                    cv.gameObject.SetActive(false);
                }
            }
            After_Die.gameObject.SetActive(true);
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
    public void Sound_Control_Human()
    {
        if (Is_Dashing)
        {
            Sound_Manager.instance.Play_sfx(Sound_Manager.instance.Dash_Sound);
        }
        else
        {
            Sound_Manager.instance.Stop_sfx(Sound_Manager.instance.Dash_Sound);
        }
    }
    public void Escape()
    {
        Option.gameObject.SetActive(!Option.gameObject.activeSelf);
        Cursor.visible = Option.gameObject.activeSelf ? true : false;
        Debug.Log("esc");
    }
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, InteractRadius);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(Landing, 0.2f);
    }
}
