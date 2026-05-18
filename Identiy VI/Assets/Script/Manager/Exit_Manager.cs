using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit_Manager : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Player_NeedSave"))
        {
            Victory_Manager.instance.Escape();
            if (collision.gameObject.TryGetComponent<AudioSource>(out AudioSource audioSource))
            {
                audioSource.mute = true;
            }
            Single_Player_UI single_Player_UI = Player_HUD_Manager.instance.player_huds[collision.gameObject];
            single_Player_UI.Player_Name.text += "\n(рялсюК)";
            Debug.Log(collision.gameObject.name+"рялсюК");
            Destroy(collision.gameObject);
        }
    }
}
