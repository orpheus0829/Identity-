using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class Cipher_Radius : MonoBehaviour
{
    public Cipher_Controller ciph;
    public Slider slider;
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Butcher") || other.CompareTag("Player"))
        {
            slider.gameObject.SetActive(true);
        }
    }
    public void FixedUpdate()
    {
        if (!ciph.is_coding)
        {
            slider.GetComponent<CanvasGroup>().alpha = 0.3f;
        }
    }
    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Butcher") || other.CompareTag("Player"))
        {
            slider.gameObject.SetActive(false);
        }
    }
}
