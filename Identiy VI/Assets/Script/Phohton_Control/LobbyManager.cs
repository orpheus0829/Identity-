using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("加载图")]
    public GameObject Loading_Image;
    [Header("左侧昵称模块")]
    public TMP_InputField nickInput;
    public const string DefaultNick = "玩家";

    [Header("中间建房模块")]
    public TMP_InputField roomNameInput;
    public Button createRoomBtn;
    public const string DefaultRoomName = "默认房间名";
    public const int MaxRoomPlayer = 2;

    [Header("右侧房间列表")]
    public Transform roomListParent;
    public GameObject roomItemPrefab;

    public List<RoomInfo> roomCache = new List<RoomInfo>();
    public List<GameObject> roomItemPool = new List<GameObject>();

    public void Awake()
    {
        Loading_Image.SetActive(true);
    }
    public void Start()
    {
        // 初始化默认名称
        nickInput.text = DefaultNick;
        roomNameInput.text = DefaultRoomName;

        // 连接服务器并进入大厅
        PhotonNetwork.ConnectUsingSettings();
        createRoomBtn.onClick.RemoveAllListeners();
        createRoomBtn.onClick.AddListener(CreateNewRoom);
    }

    // 连接服务器成功
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        Loading_Image.SetActive(false);
        Debug.Log("连接到大厅服务器");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Loading_Image.SetActive(false);
        Back_To_Hall();
        Debug.LogWarning("断开连接: " + cause);
    }

    // 实时同步玩家昵称
    public void UpdatePlayerNick()
    {
        if (!string.IsNullOrEmpty(nickInput.text))
        {
            PhotonNetwork.NickName = nickInput.text;
        }
    }

    // 创建房间
    public void CreateNewRoom()
    {
        UpdatePlayerNick();
        string roomName = roomNameInput.text;
        if (string.IsNullOrEmpty(roomName)) return;

        RoomOptions roomOpt = new RoomOptions();
        roomOpt.MaxPlayers = MaxRoomPlayer;
        roomOpt.IsOpen = true;
        roomOpt.IsVisible = true;

        Submit_Room_Info(roomOpt);

        PhotonNetwork.CreateRoom(roomName, roomOpt);
    }
    public void Submit_Room_Info(RoomOptions room_opt)
    {
        ExitGames.Client.Photon.Hashtable roomProps = new ExitGames.Client.Photon.Hashtable();
        roomProps["RoomName"] = roomNameInput.text;
        roomProps["MaxPlayers"] = MaxRoomPlayer;
        room_opt.CustomRoomProperties = roomProps;

        room_opt.CustomRoomPropertiesForLobby = new string[] { "RoomName", "MaxPlayers" };
    }

    // 房间列表刷新
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (var info in roomList)
        {
            roomCache.RemoveAll(r => r.Name == info.Name);
            if (!info.RemovedFromList)
                roomCache.Add(info);
        }
        RefreshRoomListUI();
    }

    public void RefreshRoomListUI()
    {
        foreach (var item in roomItemPool)
        {
            Destroy(item);
        }
        roomItemPool.Clear();

        foreach (var room in roomCache)
        {
            bool IsFullRoom = room.PlayerCount < room.MaxPlayers ? false : true;
            string Notice = IsFullRoom ? "(已满)" : "(可加入)";
            GameObject item = Instantiate(roomItemPrefab, roomListParent);
            item.GetComponentInChildren<TextMeshProUGUI>().text = $"{room.Name} ({room.PlayerCount}/{room.MaxPlayers}).{Notice}";
            item.GetComponent<Button>().onClick.AddListener(() =>
            {
                UpdatePlayerNick();
                PhotonNetwork.JoinRoom(room.Name);
            });
            roomItemPool.Add(item);
        }
    }

    public override void OnJoinedLobby()
    {
        Loading_Image.SetActive(false);
    }

    // 成功进入房间，跳转房间等待场景
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("RoomScene");
        Debug.Log("成功进入房间!房间名:" + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("当前房间人数:" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers);
    }

    // 加入房间失败提示
    public override void OnJoinRoomFailed(short code, string msg)
    {
        Debug.LogWarning($"加入房间失败：{msg}");
    }
    public void Back_To_Hall()
    {
        SceneManager.LoadScene(0);
    }
}