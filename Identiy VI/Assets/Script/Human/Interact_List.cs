using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_List : MonoBehaviour
{
    public CircleCollider2D circleCollider2D;
    public List<GameObject> Interact_Range;
    public readonly HashSet<string> Interactable = new HashSet<string>()
    {
        "Board",
        "Cipher",
        "Window",
        "Gate",
        "Player_NeedSave"
    };
    public void Awake()
    {
        circleCollider2D = GetComponent<CircleCollider2D>();
        circleCollider2D.isTrigger = true;
    }
    public void Start()
    {
        Player player = GetComponentInParent<Player>();
        Player_AI player_ai = GetComponentInParent<Player_AI>();
        if (player)
        {
            circleCollider2D.radius = player.InteractRadius;
        }
        else if (player_ai)
        {
            circleCollider2D.radius = player_ai.InteractRadius;
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (Interactable.Contains(collision.gameObject.tag))
        {
            Board board = collision.GetComponent<Board>();
            if (board != null && board.Current_State == Board_Style.Broken)
            {
                return;
            }
            if (!Interact_Range.Contains(collision.gameObject))
            {
                Interact_Range.Add(collision.gameObject);
            }
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (Interactable.Contains(collision.gameObject.tag))
        {
            if (Interact_Range.Contains(collision.gameObject))
            {
                Interact_Range.Remove(collision.gameObject);
            }
        }
    }
}
