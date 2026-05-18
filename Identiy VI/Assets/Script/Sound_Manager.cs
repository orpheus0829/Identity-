using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound_Manager : MonoBehaviour
{
    public static Sound_Manager instance { private set; get; }
    public AudioSource Crossing_Sound;
    [Header("人类音效")]
    public AudioSource Hurt_Sound;
    public AudioSource Dash_Sound;
    [Header("屠夫音效")]
    public AudioSource Attack_Sound;
    public AudioSource Breaking_Sound;
    //[Header("引用")]
    //public AudioSource sfx_foot;
    //public AudioSource sfx_coding;
    //public AudioSource sfx_hurt;
    //public AudioSource sfx_dash;
    //public AudioSource sfx_attack;
    //public AudioSource sfx_breaking;
    //public AudioSource sfx_crossing;

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
    public void Update()
    {
        AudioListener.pause = false;
    }
    public void Play_sfx(AudioSource audio)
    {
        if (!audio.isPlaying)
        {
            audio.Play();
            Debug.Log("播放" + audio.clip);
        }
    }
    public void Stop_sfx(AudioSource audio)
    {
        audio.Stop();
    }
    //public void PlaySFX(AudioClip clip)
    //{
    //    if (sfx && clip)
    //    {
    //        sfx.PlayOneShot(clip);
    //        Debug.Log("播放音效" + clip);
    //    }
    //}
    public void Play_Music(AudioSource source)
    {
        if (source)
        {
            source.Play();
            Debug.Log("播放音乐" + source);
        }
    }
    public void Pause_Music(AudioSource source)
    {
        if (source)
        {
            source.Pause();
            Debug.Log("暂停音乐" + source);
        }
    }
    public void Stop_Music(AudioSource source)
    {
        if (source)
        {
            source.Stop();
            Debug.Log("关闭音乐" + source);
        }
    }
    public void ALL_Music_Control(bool is_Stop)
    {
        AudioListener.pause = !is_Stop;
    }
}
