using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Cipher_Manager : MonoBehaviour
{
    public static Cipher_Manager instance { private set; get; }
    public int All_Cipher;
    public int Cipher_Count;
    public int max;
    public LayerMask cipher;
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
        Cipher_Count = 0;
    }
    public void Update()
    {
        Scan_Cipher();
    }
    public void Scan_Cipher()
    {
        Cipher_Count = 0;
        Collider2D[] col = Physics2D.OverlapCircleAll(transform.position, 100000,cipher);
        foreach(var i in col)
        {
            if (i.CompareTag("Cipher_CantCoding"))
            {
                Cipher_Count++;
            }
            max = Cipher_Count > max ? Cipher_Count : max;
        }
        if (max == All_Cipher)
        {
            Gate_Controller.instance.Final = true;
        }
    }
}
