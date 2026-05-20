using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sound_Slider : MonoBehaviour
{
    public Slider Sound;
    public void Start()
    {
        Sound.value = AudioListener.volume;
    }
    public void Set_Volume()
    {
        AudioListener.volume = Sound.value;
        Debug.Log("µ±«∞“Ù¡ø:" + AudioListener.volume);
    }
}
