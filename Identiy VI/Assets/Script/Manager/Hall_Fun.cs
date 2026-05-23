using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hall_Fun : Base_Mgr<Hall_Fun>
{
    public Transform pos;
    public GameObject aip;
    public GameObject aib;

    [Header("°´Å¥")]
    public Button CP;
    public Button CB;
    public Button DP;
    public Button DB;
    protected override void Awake()
    {
        base.Awake();
        CP.onClick.RemoveAllListeners();
        CB.onClick.RemoveAllListeners();
        DP.onClick.RemoveAllListeners();
        DB.onClick.RemoveAllListeners();

        CP.onClick.AddListener(Create_P);
        CB.onClick.AddListener(Create_B);
        DP.onClick.AddListener(Del_P);
        DB.onClick.AddListener(Del_B);
    }
    public void Create_P()
    {
        GameObject p = Instantiate(aip, pos.position,Quaternion.identity);
        Start_Hall_Camera.instance.targets.Add(p.transform);
    }
    public void Create_B()
    {
        GameObject b = Instantiate(aib, pos.position, Quaternion.identity);
        Start_Hall_Camera.instance.targets.Add(b.transform);
    }
    public void Del_P()
    {
         GameObject player_AI = GameObject.FindGameObjectWithTag("Player");
        Start_Hall_Camera.instance.targets.Remove(player_AI.transform);
        Destroy(player_AI);
    }
    public void Del_B()
    {
        GameObject butcher_AI = GameObject.FindGameObjectWithTag("Butcher_Bot");
        Start_Hall_Camera.instance.targets.Remove(butcher_AI.transform);
        Destroy(butcher_AI);
    }
}
