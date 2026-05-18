using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Settings : MonoBehaviour
{
    public static Game_Settings instance;
    public int Play_Role;
    public int AI_Player_Num;
    public int AI_Buthcer_Num;
    public void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
