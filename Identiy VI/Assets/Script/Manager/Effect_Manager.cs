using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Manager : MonoBehaviour
{
    [System.Serializable]
    public class Human_And_Effect
    {
        public GameObject Human_Being;
        public GameObject Foot;
        //public GameObject Hurt;
    }
    public static Effect_Manager instance { private set; get; }
    [Header("Ãÿ–ß")]
    public GameObject Human_Hurt;
    public GameObject Human_FootPrint;
    public LayerMask Human_Layer;
    [Header("Ω≈”°")]
    public List<Human_And_Effect> Human_lst = new List<Human_And_Effect>();
    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        Update_List();
        Playe_Human_Hurt_And_Foot();
    }
    public void Update_List()
    {
        Collider2D[] col_lst = Physics2D.OverlapCircleAll(transform.position, 10000, Human_Layer);
        Human_lst.RemoveAll(item => item.Human_Being == null);
        foreach(var i in col_lst)
        {
            bool Alive=Human_lst.Exists(item=>item.Human_Being==i.gameObject);
            if (Alive)
            {
                continue;
            }
            Human_And_Effect human_new = new Human_And_Effect
            {
                Human_Being = i.gameObject,
                Foot = null,
                //Hurt = null
            };
            Human_lst.Add(human_new);
        }
    }
    public void Playe_Human_Hurt_And_Foot()
    {
        for(int i = 0; i < Human_lst.Count; i++)
        {
            if(Human_lst[i].Human_Being.TryGetComponent(out Player pl)){
                if (pl.Hurt_VFX)
                {
                    Instantiate(Human_Hurt, pl.transform.position, Quaternion.identity);
                    pl.Hurt_VFX = false;
                }
            }
            if (Human_lst[i].Human_Being.TryGetComponent(out Player_AI pl_ai)){
                if (pl_ai.Hurt_VFX)
                {
                    Instantiate(Human_Hurt, pl_ai.transform.position, Quaternion.identity);
                    pl_ai.Hurt_VFX = false;
                }
            }
            if(Human_lst[i].Human_Being.TryGetComponent(out Rigidbody2D rb))
            {
                if (rb.velocity != Vector2.zero && Human_lst[i].Foot == null)
                {
                    GameObject Foot_Print = Instantiate(Human_FootPrint, Human_lst[i].Human_Being.transform.position, Quaternion.identity);
                    Human_lst[i].Foot = Foot_Print;
                    Foot_Print.transform.SetParent(Human_lst[i].Human_Being.transform);
                }
                else if(rb.velocity==Vector2.zero && Human_lst[i].Foot != null)
                {
                    Destroy(Human_lst[i].Foot);
                    Human_lst[i].Foot = null;
                }
            }
        }
    }
}
