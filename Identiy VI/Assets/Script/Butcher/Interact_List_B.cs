using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_List_B : MonoBehaviour
{
    public CircleCollider2D circleCollider2D_b;
    public List<GameObject> Interact_Range_B;
    public readonly HashSet<string> Interactable_B = new HashSet<string>()
    {
        "Board",
        "Window",
    };
    public void Awake()
    {
        circleCollider2D_b = GetComponent<CircleCollider2D>();
        circleCollider2D_b.isTrigger = true;
    }
    public void Start()
    {
        Butcher butcher = GetComponentInParent<Butcher>();
        Butcher_AI butcher_ai = GetComponentInParent<Butcher_AI>();
        if (butcher)
        {
            circleCollider2D_b.radius = butcher.Interact_Radius_B;
        }
        else if (butcher_ai)
        {
            circleCollider2D_b.radius = butcher_ai.Interact_Radius_B;
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (Interactable_B.Contains(collision.gameObject.tag))
        {
            if (collision.gameObject.tag == "Board")
            {
                Board b = collision.GetComponent<Board>();
                if (b != null)
                {
                    if(b.Current_State == Board_Style.Broken && b.Current_State == Board_Style.Normal)
                    {
                        b = null;
                        return;
                    }
                }
            }
            if (!Interact_Range_B.Contains(collision.gameObject))
            {
                Interact_Range_B.Add(collision.gameObject);
            }
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (Interactable_B.Contains(collision.gameObject.tag))
        {
            if (Interact_Range_B.Contains(collision.gameObject))
            {
                Interact_Range_B.Remove(collision.gameObject);
            }
        }
    }
}
