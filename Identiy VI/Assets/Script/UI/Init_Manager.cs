using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Init_Manager : MonoBehaviour
{
    public static Init_Manager instance { private set; get; }
    public List<GameObject> targets;
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
    }
    public void Start()
    {
        Init_Player_Alive();
    }
    public void Init_Player_Alive()
    {
        int layer = LayerMask.NameToLayer("Player");
        List<GameObject> targets = new List<GameObject>();
        foreach (var obj in FindObjectsByType<GameObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            if (obj.layer == layer && (obj.GetComponent<Player>() || obj.GetComponent<Player_AI>()))
            {
                targets.Add(obj);
                Debug.Log("找到玩家/AI: " + obj.name);
            }
        }
        Debug.Log("一共找到: " + targets.Count + " 个角色");

        List<Player_HUD_Manager.Player_Init> list = new List<Player_HUD_Manager.Player_Init>();
        foreach (var go in targets)
        {
            list.Add(new Player_HUD_Manager.Player_Init
            {
                player_name = go.name,
                player_obj = go
            });
            Debug.Log("找到玩家/AI: " + new Player_HUD_Manager.Player_Init { player_name = go.name });
        }
        Debug.Log("一共找到: " + list.Count + " 个角色");
        Player_HUD_Manager.instance.Refresh_Player_HUD(list);
    }
}