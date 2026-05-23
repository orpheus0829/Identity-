using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-999)]
public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    public static PlayerSpawner instance { private set; get; }

    public GameObject humanPrefab;
    public GameObject butcherPrefab;
    public List<Transform> Spawns;
    public GameObject myCharacter;

    public CinemachineVirtualCamera v;

    public int Spawn_Num = 0;


    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        GameObject camera_v = GameObject.FindGameObjectWithTag("Cinemachine_Camera");
        if (camera_v != null)
        {
            v = camera_v.GetComponent<CinemachineVirtualCamera>();
        }
        else
        {
            Debug.Log("找不到相机");
            v = null;
        }
        GameObject[] point = GameObject.FindGameObjectsWithTag("Spawn_Point");
        foreach (var i in point)
        {
            Spawns.Add(i.transform);
        }
    }
    public void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Spawn_Num = Random.Range(0, Spawns.Count);
        }
        else
        {
            Spawn_Num = Random.Range(Spawns.Count / 2, Spawns.Count);
        }
        SpawnMyRole();
    }

    public void SpawnMyRole()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            myCharacter = PhotonNetwork.Instantiate(butcherPrefab.name, Spawns[Spawn_Num].position, Quaternion.identity);
        }
        else
        {
            myCharacter = PhotonNetwork.Instantiate(humanPrefab.name, Spawns[Spawn_Num].position, Quaternion.identity);
        }
        if (v != null && myCharacter != null)
        {
            v.Follow = myCharacter.transform;
        }
    }
    public void Update()
    {
        PhotonNetwork.NetworkingClient.Service();
        if (PhotonNetwork.NetworkingClient.State == ClientState.Disconnected)
        {
            return;
        }
        if (v == null || myCharacter == null)
        {
            if (myCharacter == null)
            {
                if (PhotonNetwork.InRoom)
                {
                    PhotonNetwork.LeaveRoom();
                }
                SceneManager.LoadScene(0);
            }
            return;
        }
        if (v.Follow == null)
        {
            v.Follow = myCharacter.transform;
        }
    }
}