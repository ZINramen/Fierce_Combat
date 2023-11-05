using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using HashTable = ExitGames.Client.Photon.Hashtable;

enum ButtonNumber
{
    PreviousNumber = -2,
    NextBtnNumber = -1
};

enum RoomIndex
{
    ZeroRoom    = 0,
    FirstRoom   = 1,
    SecondRoom  = 2,
    ThirdRoom   = 3
};

public class PhotonInit : MonoBehaviourPunCallbacks
{
    // �̱��� ������ Ȱ���Ͽ�, ���� ���� & �ı�x
    public static PhotonInit instance;
    
    private bool isGameStart    = false;
    private bool isLogin        = false;
    
    private string  playerName      = "";
    private string  connectionState = "";
    public  string  chatMessage;

    public  InputField  playerInput;
    private ScrollRect  scroll_rect = null;
    private PhotonView  pv;

    private Text        chatText;
    private Text        connectionInfo_Text;

    [Header("LobbyCanvas")]
    public GameObject LobbyCanvas;
    
    public GameObject LobbyPanel;
    public GameObject RoomPanel;
    public GameObject MakeRoomPanel;

    public InputField RoomInput;
    public InputField RoomPwInput;

    public Toggle PwToggle;
    public GameObject PwPanel;
    public GameObject PwErrorLog;
    public GameObject PwConfirmBtn;
    public GameObject PwPanelClose_Btn;
    public InputField Pw_CheckInput;

    public bool LockSate = false;
    public string privateRoom;
    
    public Button[] CellBtn;

    public Button PreviousBtn;
    public Button NextBtn;
    public Button CreateRoomBtn;

    public int hashTalbeCount;

    private List<RoomInfo> myList = new List<RoomInfo>();
    private int currPage    = 1;
    private int maxPage     = 0;
    private int multiple    = 0;
    private int roomNumber  = 0;

    private void Awake()
    {
        PhotonNetwork.GameVersion = "MyFps 1.0";
        PhotonNetwork.ConnectUsingSettings();

        if (GameObject.Find("ChatText") != null)
            chatText =  GameObject.Find("ChatText").GetComponent<Text>();

        if (GameObject.Find("Scroll View") != null)
            scroll_rect = GameObject.Find("Scroll View").GetComponent<ScrollRect>();

        if (GameObject.Find("ConnectionInfoText") != null)
            connectionInfo_Text = GameObject.Find("ConnectionInfoText").GetComponent<Text>();

        connectionState = "������ ������ ���� ��...";

        if (connectionInfo_Text)
            connectionInfo_Text.text = connectionState;

        //�Ʒ��� �Լ��� ����Ͽ� ���� ��ȯ �Ǵ��� ���� �Ǿ��� �ν��Ͻ��� �ı����� �ʴ´�.
        //DontDestroyOnLoad(gameObject);
    }

    // ���� ������Ʈ �� ���� ȣ��Ǵ� �Լ�
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("OnRoomListUpdate" + roomList.Count);
        int roomCount = roomList.Count;

        for (int i = 0; i < roomCount; i++) 
        {
            // ����Ʈ�� �����Ѵٸ�
            if (!roomList[i].RemovedFromList)
            {
                // �� ����Ʈ�� �������� �ʴ´ٸ�, �߰����ش�.
                if (!myList.Contains(roomList[i])) 
                    myList.Add(roomList[i]);

                // �� ����Ʈ�� �����Ѵٸ�, roomList�� �����ϴ� ����Ʈ�� �ε����� �����ͼ� �����͸� �־��ش�.
                else
                    myList[myList.IndexOf(roomList[i])] = roomList[i];
            }

            // ����Ʈ�� �������� �ʰ�, �� ����Ʈ���� �����Ѵٸ�, ������ ������ �����Ͽ�, ����
            else if (myList.IndexOf(roomList[i]) != -1) 
                myList.RemoveAt(myList.IndexOf(roomList[i]));
        }

        MyListRenewal();
    }

    public void MyListClick(int btn_Num)
    {
        if(btn_Num == (int)ButtonNumber.PreviousNumber)
        {
            --currPage;
            MyListRenewal();
        }

        else if (btn_Num == (int)ButtonNumber.NextBtnNumber)
        {
            ++currPage;
            MyListRenewal();
        }

        // Ŭ���� ���� �н����尡 �ִٸ�, �е���� â ����
        else if (myList[multiple + btn_Num].CustomProperties["password"] != null)
        {
            PwPanel.SetActive(true);
        }

        // ���ٸ�, �濡 ����
        else
        {
            PhotonNetwork.JoinRoom(myList[multiple + btn_Num].Name);
            MyListRenewal();
        }
    }

    // �� �ε����� ��ȯ�ϴ� �Լ�
    public void RoomPw(int roomIndex)
    {
        switch(roomIndex)
        {
            case (int)RoomIndex.ZeroRoom:
                roomIndex = (int)RoomIndex.ZeroRoom;
                break;

            case (int)RoomIndex.FirstRoom:
                roomIndex = (int)RoomIndex.FirstRoom;
                break;

            case (int)RoomIndex.SecondRoom:
                roomIndex = (int)RoomIndex.SecondRoom;
                break;

            case (int)RoomIndex.ThirdRoom:
                roomIndex = (int)RoomIndex.ThirdRoom;
                break;

            default:
                break;
        }
    }

    // �н� ���尡 �ִ� �濡 ���� ��, ����� �°� Ʋ�� ������ ����
    public void EnterRoomWithPW()
    {
        if ((string)myList[multiple + roomNumber].CustomProperties["password"] == Pw_CheckInput.text)
        {
            PhotonNetwork.JoinRoom(myList[multiple + roomNumber].Name);
            MyListRenewal();
            PwPanel.SetActive(false);
        }
        else
            StartCoroutine("ShowPwWrongMsg");
    }

    IEnumerator ShowPwWrongMsg()
    {
        if (PwErrorLog.activeSelf)
        {
            PwErrorLog.SetActive(true);
            yield return new WaitForSeconds(3.0f);
            PwErrorLog.SetActive(false);
        }
    }

    public void MyListRenewal()
    {
        maxPage = (myList.Count % CellBtn.Length == 0) ? 
            myList.Count / CellBtn.Length : 
            myList.Count / CellBtn.Length + 1;

        Debug.Log("Max Page = " + maxPage + "\n" + "CellBtn : " + CellBtn.Length);

        PreviousBtn.interactable = (currPage <= 1) ? false : true;
        NextBtn.interactable = (currPage >= maxPage) ? false : true;

        multiple = (currPage - 1) * CellBtn.Length;

        for (int i = 0; i < CellBtn.Length; i++)
        {
            CellBtn[i].interactable = (multiple + i < myList.Count) ? true : false;
            CellBtn[i].transform.GetChild(0).GetComponent<Text>().text =
                (multiple + i < myList.Count ? myList[multiple + i].Name : "");

            CellBtn[i].transform.GetChild(1).GetComponent<Text>().text =
                (multiple + i < myList.Count) ?
                myList[multiple + i].PlayerCount + "/" + myList[multiple + i].MaxPlayers : "";
        }
    }

    //public override void OnJoinedLobby()
    //{
    //    base.OnJoinedLobby();
    //    Debug.Log("Joined Lobby");
    //    //PhotonNetwork.CreateRoom("MyRoom");
    //    PhotonNetwork.JoinRandomRoom();
    //    //PhotonNetwork.JoinRoom("MyRoom");
    //}

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            connectionState = "�뿡 ����...";

            if (connectionInfo_Text)
                connectionInfo_Text.text = connectionState;

            LobbyPanel.SetActive(false);
            RoomPanel.SetActive(true);
            PhotonNetwork.JoinLobby();
            //PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            connectionState = "�������� : ������ ������ ������� ����\n���� ��õ���...";

            if (connectionInfo_Text)
                connectionInfo_Text.text = connectionState;

            PhotonNetwork.ConnectUsingSettings();
        }

    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        connectionState = "No Room";

        if (connectionInfo_Text)
            connectionInfo_Text.text = connectionState;

        Debug.Log("No Room");
        //PhotonNetwork.CreateRoom("MyRoom");
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        connectionState = "Finish make a room";

        if (connectionInfo_Text)
            connectionInfo_Text.text = connectionState;

        Debug.Log("Finish make a room");
        
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log("OnCreateRoomFailed:"+returnCode + "-"+message);
    }
   
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        connectionState = "Joined Room";

        if (connectionInfo_Text)
            connectionInfo_Text.text = connectionState;

        Debug.Log("Joined Room");
        isLogin = true;
        PlayerPrefs.SetInt("LogIn", 1);

        //StartCoroutine(CreatePlayer());
        PhotonNetwork.LoadLevel("SampleScene");
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("LogIn", 0); 
    }

    private void Update()
    {
        if (PlayerPrefs.GetInt("LogIn") == 1)
            isLogin = true;

        if(isGameStart == false && SceneManager.GetActiveScene().name == "SampleScene" && isLogin == true)
        {
            isGameStart = true;
            if (GameObject.Find("ChatText") != null)
                chatText = GameObject.Find("ChatText").GetComponent<Text>();

            if (GameObject.Find("Scroll View") != null)
                scroll_rect = GameObject.Find("Scroll View").GetComponent<ScrollRect>();

            StartCoroutine(CreatePlayer());
        }
    }

    IEnumerator CreatePlayer()
    {
        while(!isGameStart)
        {
            yield return new WaitForSeconds(0.5f);
        }

        GameObject tempPlayer = PhotonNetwork.Instantiate("PlayerDagger",
                                    new Vector3(0, 0, 0),
                                    Quaternion.identity,
                                    0);
        tempPlayer.GetComponent<PlayerCtrl>().SetPlayerName(playerName);
        pv = GetComponent<PhotonView>();

        yield return null;
    }

    private void OnGUI()
    {
        GUILayout.Label(connectionState);
    }

    public void SetPlayerName()
    {
        Debug.Log(playerInput.text + "�� �Է� �ϼ̽��ϴ�!");

        // Lobby��
        if (isGameStart == false && isLogin == false)
        {
            playerName = playerInput.text;
            playerInput.text = string.Empty;
            Debug.Log("Connect �õ�!" + isGameStart + ", " + isLogin);
            Connect();
        }

        //Set Push�� ���
        else
        {
            chatMessage = playerInput.text;
            playerInput.text = string.Empty;
            //ShowChat(chatMessage);
            pv.RPC("ChatInfo", RpcTarget.All, chatMessage);
        }
        
    }

    public void ShowChat(string chat)
    {
        chatText.text += chat + "\n";
        scroll_rect.verticalNormalizedPosition = 0.0f;
    }

    [PunRPC]
    public void ChatInfo(string sChat)
    {
        ShowChat(sChat);
    }

    public void CreateRoom_BtnClick()
    {
        RoomPanel.SetActive(false);
        MakeRoomPanel.SetActive(true);
    }

    public void MakeOK_BtnOnClick()
    {
        MakeRoomPanel.SetActive(false);
    }

    public void DisConnect_BtnClick()
    {
        PhotonNetwork.Disconnect();

        RoomPanel.SetActive(false);
        LobbyPanel.SetActive(true);

        connectionState = "������ ������ ���� ��...";
        if (connectionInfo_Text)
            connectionInfo_Text.text = connectionState;

        isGameStart = false;
        isLogin = false;
        PlayerPrefs.SetInt("LogIn", 0);
    }

    public void CreateNewRoom()
    {
        Debug.Log("�� ���� ��ư Ŭ��");
        RoomOptions roomOptions = new RoomOptions();

        roomOptions.MaxPlayers = 20;
        roomOptions.CustomRoomProperties = new HashTable()
        {
            { "password", RoomInput.text}
        };

        roomOptions.CustomRoomPropertiesForLobby = new string[] { "password" };

        if(PwToggle.isOn)
            PhotonNetwork.CreateRoom(RoomInput.text == "" ? "Game" + Random.Range(0, 80) : "*" + RoomInput.text, 
                roomOptions);

        else
            PhotonNetwork.CreateRoom(RoomInput.text == "" ? "Game" + Random.Range(0, 80) : RoomInput.text, 
                new RoomOptions { MaxPlayers = 20 });

        MakeRoomPanel.SetActive(false);
    }
}
