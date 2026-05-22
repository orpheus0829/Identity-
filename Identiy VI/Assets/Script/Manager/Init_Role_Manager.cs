using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Init_Role_Manager : MonoBehaviour
{
    public List<Transform> Spawn_Points;
    public bool[] Has_Spawned;
    public int Role;
    public GameObject Prefab_Human;
    //public int Target_Human_Num;
    public GameObject Prefab_Butcher;
    //public int Target_Butcher_Num;
    public GameObject Prefab_Human_AI;
    public int Target_AI_Human_Num;
    public GameObject Prefab_Butcher_AI;
    public int Target_AI_Butcher_Num;
    public List<GameObject> Prefab_Pool;
    public CinemachineVirtualCamera camera_v;
    public GameObject Player_UI_Panel;
    public void Awake()
    {
        Screen.SetResolution(1920, 1080, true);
        Screen.fullScreen = true;

        GameObject vCam = GameObject.FindGameObjectWithTag("Cinemachine_Camera");
        camera_v = vCam.GetComponent<CinemachineVirtualCamera>();
        camera_v.m_Lens.OrthographicSize = 6;
        Target_AI_Human_Num = Game_Settings.instance.AI_Player_Num;
        Target_AI_Butcher_Num = Game_Settings.instance.AI_Buthcer_Num;
        Role = Game_Settings.instance.Play_Role;
        GameObject[] Spawns = GameObject.FindGameObjectsWithTag("Spawn_Point");
        foreach(var i in Spawns)
        {
            Spawn_Points.Add(i.transform);
        }
        Has_Spawned = new bool[Spawn_Points.Count];
        for (int j=0;j<Spawn_Points.Count;j++)
        {
            Has_Spawned[j] = false;
        }
        if (Role == 1)
        {
            Prefab_Pool.Add(Prefab_Human);
            Player_UI_Panel.gameObject.SetActive(true);
        }
        if (Role == 2)
        {
            Prefab_Pool.Add(Prefab_Butcher);
            Player_UI_Panel.gameObject.SetActive(false);
        }
        for (int a = 0; a < Target_AI_Human_Num; a++)
        {
            Prefab_Pool.Add(Prefab_Human_AI);
        }
        for (int b = 0; b < Target_AI_Butcher_Num; b++)
        {
            Prefab_Pool.Add(Prefab_Butcher_AI);
        }
        while (Prefab_Pool.Count > 0)
        {
            bool found = false;
            for (int i = 0; i < Spawn_Points.Count; i++)
            {
                if (Has_Spawned[i] == false && Prefab_Pool.Count > 0)
                {
                    found = true;
                    int number = Random.Range(0, Prefab_Pool.Count);
                    GameObject spawnedObj = Instantiate(Prefab_Pool[number], Spawn_Points[i].position, Spawn_Points[i].rotation);
                    spawnedObj.name = spawnedObj.name.Replace("(Clone)", "");
                    if (Prefab_Pool[number] == Prefab_Human || Prefab_Pool[number] == Prefab_Butcher)
                    {
                        camera_v.Follow = spawnedObj.transform;
                    }
                    if (Role==1&& Prefab_Pool[number] == Prefab_Human)
                    {
                        Player player = spawnedObj.GetComponent<Player>();
                        Player_UI.instance.pl = player;
                    }
                    Prefab_Pool.Remove(Prefab_Pool[number]);
                    Has_Spawned[i] = true;
                }
            }
            if (!found) break;
        }
    }
    public void Start()
    {
        for (int i = 0; i < Spawn_Points.Count; i++)
        {
            Debug.Log($"索引{i} 出生点名字：{Spawn_Points[i].name},是否生成：{Has_Spawned[i]}");
        }
        Cursor.visible = false;
    }
}
