using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound_Manager : Base_Mgr<Sound_Manager>
{
    public AudioSource Crossing_Sound;
    [Header("»À¿‡“Ù–ß")]
    public AudioSource Hurt_Sound;
    public AudioSource Dash_Sound;
    [Header("Õ¿∑Ú“Ù–ß")]
    public AudioSource Attack_Sound;
    public AudioSource Breaking_Sound;
    protected override void Awake()
    {
        base.Awake();
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
            Debug.Log("≤•∑≈" + audio.clip);
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
    //        Debug.Log("≤•∑≈“Ù–ß" + clip);
    //    }
    //}
    public void Play_Music(AudioSource source)
    {
        if (source)
        {
            source.Play();
            Debug.Log("≤•∑≈“Ù¿÷" + source);
        }
    }
    public void Pause_Music(AudioSource source)
    {
        if (source)
        {
            source.Pause();
            Debug.Log("‘›Õ£“Ù¿÷" + source);
        }
    }
    public void Stop_Music(AudioSource source)
    {
        if (source)
        {
            source.Stop();
            Debug.Log("πÿ±’“Ù¿÷" + source);
        }
    }
    public void ALL_Music_Control(bool is_Stop)
    {
        AudioListener.pause = !is_Stop;
    }
}
