using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Woods_Enter : MonoBehaviour
{
    public readonly HashSet<string> Enter_Tag = new HashSet<string>()
    {
        "Player",
        "Butcher",
    };
    [Range(0f, 1f)] public float alpha;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (Enter_Tag.Contains(collision.gameObject.tag))
        {
            SpriteRenderer sr = collision.GetComponent<SpriteRenderer>();
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (Enter_Tag.Contains(collision.gameObject.tag))
        {
            SpriteRenderer sr = collision.GetComponent<SpriteRenderer>();
            Color c = sr.color; 
            c.a = 1f;
            sr.color = c;
        }
    }
}
