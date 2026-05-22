using Cinemachine;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-999)]
public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    public GameObject humanPrefab;
    public GameObject butcherPrefab;
    public List<Transform> Spawns;
    public GameObject myCharacter;

    public CinemachineVirtualCamera v;

    public int Spawn_Num = 0;

    public void Awake()
    {
        GameObject camera_v = GameObject.FindGameObjectWithTag("Cinemachine_Camera");
        v = camera_v.GetComponent<CinemachineVirtualCamera>();
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
        v.Follow = myCharacter.transform;
    }
    public void Update()
    {
        if (v == null)
        {
            return;
        }
        if (v.Follow == null)
        {
            v.Follow = myCharacter.transform;
        }
    }
}