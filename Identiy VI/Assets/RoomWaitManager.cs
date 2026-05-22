using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class RoomWaitManager : MonoBehaviourPunCallbacks
{
    [Header("房间信息")]
    public TextMeshProUGUI Room_Info;
    [Header("玩家列表")]
    public Transform playerListParent;
    public GameObject playerItemPrefab;

    [Header("功能按钮")]
    public Button startGameBtn;
    public Button leaveRoomBtn;

    public List<GameObject> playerItemPool = new List<GameObject>();
    public readonly int FullPlayerNum = 2;

    public override void OnJoinedRoom()
    {
        RefreshPlayerList();
        CheckStartBtnCondition();
        Read_Room_Info();
    }
    public void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();

        RefreshPlayerList();
        CheckStartBtnCondition();
        leaveRoomBtn.onClick.RemoveAllListeners();
        leaveRoomBtn.onClick.AddListener(ExitRoom);

        Read_Room_Info();


    }

    public void Read_Room_Info()
    {
        if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.CustomProperties == null)
        {
            return;
        }

        var props = PhotonNetwork.CurrentRoom.CustomProperties;

        if (props.ContainsKey("RoomName") && props.ContainsKey("MaxPlayers"))
        {
            string roomName = (string)props["RoomName"];
            int maxPlayers = (int)props["MaxPlayers"];
            Room_Info.text = $"{roomName}({PhotonNetwork.CurrentRoom.PlayerCount}/{maxPlayers})";
        }
    }

    // 刷新房间成员
    public void RefreshPlayerList()
    {
        foreach (var item in playerItemPool) Destroy(item);
        playerItemPool.Clear();

        foreach (var player in PhotonNetwork.PlayerList)
        {
            GameObject item = Instantiate(playerItemPrefab, playerListParent);
            string roleTag = PhotonNetwork.IsMasterClient ? "屠夫(房主)" : "逃生者(访客)";
            // 固定身份标识
            if (player.IsMasterClient)
            {
                item.GetComponentInChildren<TextMeshProUGUI>().text = $"{player.NickName} | 屠夫";
            }
            else
            {
                item.GetComponentInChildren<TextMeshProUGUI>().text = $"{player.NickName} | 逃生者";
            }
            playerItemPool.Add(item);
        }
    }

    // 校验开局条件：仅房主可见按钮，且必须满2人
    public void CheckStartBtnCondition()
    {
        bool isMaster = PhotonNetwork.IsMasterClient;
        bool isFull = PhotonNetwork.CurrentRoom.PlayerCount >= FullPlayerNum;
        startGameBtn.gameObject.SetActive(isMaster && isFull);
    }

    // 房主开启游戏
    public void OnClickStartGame()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        // 锁定房间禁止新人加入
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        // 同步跳转游戏场景
        PhotonNetwork.LoadLevel("Multi_Map");
    }

    // 退出房间返回大厅
    public void ExitRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        RefreshPlayerList();
        CheckStartBtnCondition();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player oldPlayer)
    {
        RefreshPlayerList();
        CheckStartBtnCondition();
    }

    // 离开房间回调，切回大厅
    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("Lobby");
    }
}