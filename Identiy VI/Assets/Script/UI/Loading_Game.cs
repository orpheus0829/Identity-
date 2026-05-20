using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading_Game : MonoBehaviour
{
    public static Loading_Game instance { private set; get; }
    public float Gap_1;
    public float Gap_2;
    public Slider Loading_Slider;
    public TextMeshProUGUI Percent;
    public Button Mode1;
    public Button Mode2;
    public float Loading_Speed;
    public bool Is_Start;
    public void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        Gap_1 = Random.Range(1, 30);
        Gap_2 = Random.Range(Gap_1, 100);
        Is_Start = false;
    }
    //public void Start_Loading()
    //{
    //    StartCoroutine(Loading());
    //}
    //public IEnumerator Loading()
    //{
    //    Loading_Slider.value += Time.deltaTime * Loading_Speed;
    //    Loading_Speed = (Loading_Slider.value < Gap_2 && Loading_Slider.value > Gap_1) ? 0.5f : 1f;
    //    yield return null;
    //}
    public void Update()
    {
        if (Is_Start)
        {
            Loading_Speed = (Loading_Slider.value < Gap_2 && Loading_Slider.value > Gap_1) ? 0.005f : 1f;
            Loading_Slider.value += Time.deltaTime * Loading_Speed;
            Percent.text = $"{Loading_Slider.value * 100f:0}%";
        }
        if (Loading_Slider.value == Loading_Slider.maxValue)
        {
            Is_Start = false;
            Loading_Slider.value = 0;
            SceneManager.LoadScene(2);
            Loading_Slider.value = 0;
        }
    }
}
