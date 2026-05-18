using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Cipher_Controller : MonoBehaviour
{
    public CipherData cipher_data;
    public float Cipher_Need;
    [Header("可破译范围")]
    public float Cipher_away_radius;
    [Header("可显示进度条的半径")]
    public float Cipher_Slider_Radius;
    [Header("破译")]
    public float Progress;
    public bool is_coding;
    public bool Done;
    public float Calibration_Probability;
    public bool Has_Real_Player;
    //public int Coding_members;
    [Header("校准")]
    public bool CanStart;
    public bool Is_Calibration;
    public bool CanCheckCalibration = true;
    [Header("引用")]
    public Coroutine Calibration_Progress;
    public Animator animator;
    //public Player pl;
    public Slider slider;
    public GameObject Show;
    public GameObject Calibration_Sign;
    public Slider Calibration_Slider;
    public List<MonoBehaviour> Coding_Guys;
    public void Awake()
    {
        animator = GetComponent<Animator>();
        Cipher_away_radius = cipher_data.Cipher_Interact_Radius;
        Cipher_Slider_Radius = cipher_data.Cipher_Slider_Show_Radius;
        Cipher_Need = cipher_data.Cipher_Need_INT;
        Calibration_Probability = cipher_data.Calibration_Random;
        slider.maxValue = Cipher_Need;
        //Coding_members = 0;
    }
    public void Update()
    {
        //IsSliderShow();
        if (Done)
        {
            tag = "Cipher_CantCoding";
            slider.value = slider.maxValue;
            animator.Play("Cipher_Done");
        }
    }
    public void FixedUpdate()
    {
        Progress = slider.value * 100 / slider.maxValue;
        Coding_And_QTE();
        if (Coding_Guys.Count > 0)
        {
            animator.SetBool("Cipher_IsWorking", true);
        }
        else
        {
            animator.SetBool("Cipher_IsWorking", false);
        }
    }
    public void Coding_And_QTE()
    {
        if (Coding_Guys.Count <= 0)
        {
            is_coding = false;
            CanCheckCalibration = false;
            Is_Calibration = false;
            return;
        }
        if (Is_Calibration)
        {
            Calibration_Slider.value -= Time.fixedDeltaTime * 2f;
        }
        if (slider.maxValue == slider.value)
        {
            is_coding = false;
            Done = true;
            foreach (MonoBehaviour guy in Coding_Guys)
            {
                if (guy is Player_AI ai)
                {
                    ai.CanControl = true;
                    ai.Last_Cipher_Scan();
                    ai.Coding_Exit();
                }
                else if (guy is Player p)
                {
                    p.CanControl = true;
                    p.interact_List.Interact_Range.Remove(this.gameObject);
                }
            }
            Coding_Guys.Clear();
            //Coding_members = 0;
            animator.SetBool("Cipher_Success", true);
            StopCoroutine(Calibration_Reaction());
            Is_Calibration = false;
            Calibration_Sign.SetActive(false);
            Calibration_Slider.gameObject.SetActive(false);
            CanCheckCalibration = false;
            gameObject.tag = "Cipher_CantCoding";
        }
        if (Coding_Guys.Count>0)
        {
            coding();
        }
        else
        {
            if (!Done)
            {
                gameObject.tag = "Cipher";
            }
        }
    }
    public void coding()
    {
        float coding_speed_now = 0;
        foreach(MonoBehaviour guys in Coding_Guys)
        {
            if(guys is Player p_r)
            {
                Has_Real_Player = true;
                coding_speed_now += p_r.Code_Speed;
            }
            if(guys is Player_AI p_a)
            {
                coding_speed_now += p_a.Code_Speed;
            }
        }
        CanStart = (slider.value / slider.maxValue <= 0.1f ? false : true) && Has_Real_Player;
        slider.GetComponent<CanvasGroup>().alpha = 1f;
        if (CanCheckCalibration && !Is_Calibration && CanStart)
        {
            Calibration_Chose();
        }
        //slider.gameObject.transform.position = transform.position + Vector3.down * 0.5f;
        slider.value += Time.fixedDeltaTime * coding_speed_now;
    }
    public IEnumerator Calibration_Gap()
    {
        yield return new WaitForSeconds(2f);
        CanCheckCalibration = true;
    }
    public IEnumerator Calibration_Reaction()
    {
        yield return new WaitForSeconds(1f);
        Is_Calibration = true;
        Calibration_Slider.gameObject.SetActive(true);
        Calibration_Slider.value = 50f;
        yield return new WaitForSeconds(2f);
        Check_Calibration_Result();
    }
    public void Calibration_Chose()
    {
        CanCheckCalibration = false;
        Calibration_Sign.SetActive(false);
        //Calibration_Slider.gameObject.SetActive(false);
        StartCoroutine(Calibration_Gap());
        float ramdom_Coding = Random.Range(0f, 100f);
        Debug.Log(ramdom_Coding);
        if (ramdom_Coding <= Calibration_Probability && !Is_Calibration)
        {
            Calibration_Sign.SetActive(true);
            Calibration_Progress= StartCoroutine(Calibration_Reaction());
        }
        else
        {
            Calibration_Sign.SetActive(false);
            Is_Calibration = false;
        }
    }
    public void Check_Calibration_Result()
    {
        if (Calibration_Slider.value <= 0f)
        {
            Punish();
        }
        else
        {

        }
        CanCheckCalibration = true;
        Calibration_Sign.gameObject.SetActive(false);
        Calibration_Slider.gameObject.SetActive(false);
        Is_Calibration = false;
    }
    public void Punish()
    {
        slider.value -= 0.3f * slider.maxValue;
    }
    #region 交互
    public void Interact_Cipher_Player_Real(Player p)
    {
        if (Done || Coding_Guys.Contains(p))
        {
            return;
        }
        p.ciph = this;
        Vector2 Cipher_position = transform.position;
        Vector2 OfCipher = (p.transform.position - transform.position).normalized;
        p.transform.position = Cipher_position + OfCipher * Cipher_away_radius;
        p.Coding_Enter();
        is_coding = true;
        if (transform.position.x >= p.transform.position.x)
        {
            p.sr.flipX = false;
        }
        else
        {
            p.sr.flipX = true;
        }
    }
    public void Interact_Cipher_Player_AI(Player_AI ai)
    {
        if (Done || Coding_Guys.Contains(ai))
        {
            return;
        }
        ai.ciph = this;
        ai.CanControl = false;
        Vector2 Cipher_position = transform.position;
        Vector2 OfCipher = (ai.transform.position - transform.position).normalized;
        ai.transform.position = Cipher_position + OfCipher * Cipher_away_radius * 1.3f;
        ai.Coding_Enter();
        is_coding = true;
        if (transform.position.x >= ai.transform.position.x)
        {
            ai.sr.flipX = false;
        }
        else
        {
            ai.sr.flipX = true;
        }
    }
    #endregion
}
