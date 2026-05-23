using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Looby_Entrance : MonoBehaviour
{
    public Button Looby_Button;
    public void Awake()
    {
        Looby_Button = GetComponent<Button>();
        Looby_Button.onClick.RemoveAllListeners();
        Looby_Button.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(3);
        });
    }
}
