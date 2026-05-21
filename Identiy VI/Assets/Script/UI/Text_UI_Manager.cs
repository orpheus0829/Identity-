using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Text_UI_Manager : Base_Mgr<Text_UI_Manager>
{
    public TextMeshProUGUI Cipher;
    public TextMeshProUGUI Gate;
    public TextMeshProUGUI Escape_Member_Target;
    public TextMeshProUGUI Dead_Member_Target;
    public int remaining_cipher;
    protected override void Awake()
    {
        base.Awake();
    }
    public void Update()
    {
        remaining_cipher = Cipher_Manager.instance.All_Cipher - Cipher_Manager.instance.max;
        Cipher.text = $"剩余密码机:{remaining_cipher}";
        Gate.text = !Gate_Controller.instance.Final ? "大门不可开启" : Gate_Controller.instance.Finish ? "大门已开启" : "大门可开启";
        Escape_Member_Target.text = $"目标逃离人数:{Victory_Manager.instance.Escape_Member}/{Victory_Manager.instance.Target_Escape_Member}";
        Dead_Member_Target.text = $"目标击碎人数:{Victory_Manager.instance.Dead_Member}/{Victory_Manager.instance.Target_Dead_Member}";
    }
}
