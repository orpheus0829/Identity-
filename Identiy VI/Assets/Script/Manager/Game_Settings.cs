using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Settings : Base_Mgr<Game_Settings>
{
    public int Play_Role;
    public int AI_Player_Num;
    public int AI_Buthcer_Num;
    protected override void Awake()
    {
        base.Awake();
        if (instance == this)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
