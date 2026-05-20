using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Back_Button : MonoBehaviour
{
    public GameObject Parent_Panel;
    public void Enter_Back_Button()
    {
        Parent_Panel.gameObject.SetActive(false);
    }
    public void Return_To_Hall()
    {
        SceneManager.LoadScene(0);
    }
}
