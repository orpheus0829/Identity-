using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_HUD_Manager : MonoBehaviour
{
    public static Player_HUD_Manager instance { private set; get; }
    [System.Serializable]
    public class Player_Init
    {
        public string player_name;
        public GameObject player_obj;
    }
    [Header("HUDÃÂ–Õ")]
    public float HUD_Width;
    public float Spacing;
    public float baseX = -3799.5f;
    public float baseY = -2109.7f;
    [Header("…Ë÷√")]
    public Single_Player_UI Hud_Prefab;
    public Transform Hud_Containers;
    public Dictionary<GameObject, Single_Player_UI> player_huds = new Dictionary<GameObject, Single_Player_UI>();
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
    public void Refresh_Player_HUD(List<Player_Init> players)
    {
        Clear_Old_HUD();
        for(int i = 0; i < players.Count; i++)
        {
            var p = players[i];
            var New_HUD = Instantiate(Hud_Prefab, Hud_Containers);
            New_HUD.Init(p.player_name);
            float xPos = baseX - i * (HUD_Width + Spacing);
            New_HUD.transform.localPosition = new Vector3(xPos, baseY, 0f);
            player_huds.Add(p.player_obj, New_HUD);
        }
    }
    public void Update_Player_HP(GameObject playerobj,float current_hp,float maxHp)
    {
        if(player_huds.TryGetValue(playerobj,out var hud))
        {
            float hp_percent = 1 - current_hp / maxHp;
            hud.Update_HP(hp_percent);
        }
    }
    public void Clear_Old_HUD()
    {
        foreach(var hud in player_huds.Values)
        {
            Destroy(hud.gameObject);
        }
        player_huds.Clear();
    }
}
