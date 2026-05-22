using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit_Button : MonoBehaviour
{
    public void On_Click_Exit()
    {
        Debug.Log("ÍË³öÓÎÏ·");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    public void Quit_To_Hall()
    {
        SceneManager.LoadScene(0);
    }
}
