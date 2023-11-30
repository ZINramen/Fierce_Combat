using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonPlayerNetwork : MonoBehaviourPunCallbacks
{
    public DynamicCamera cam;
    public bool isLobby = false;

    [SerializeField] GameObject Player1;
    [SerializeField] GameObject Player2;
    [SerializeField] GameObject PlayerLobby;

    PhotonView pv;

    public void StageLoad(string name)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(name);
        }
    }

    private void Start()
    {
        DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
        if (!pool.ResourceCache.ContainsKey(Player1.name))
        {
            pool.ResourceCache.Add(Player1.name, Player1);
            pool.ResourceCache.Add(Player2.name, Player2);
            pool.ResourceCache.Add(PlayerLobby.name, PlayerLobby);
        }
        if (!isLobby)
        {
            cam = Camera.main.GetComponent<DynamicCamera>();
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Instantiate(Player1.name, Vector3.zero, Quaternion.identity);
            }
            else
            {
                PhotonNetwork.Instantiate(Player2.name, Vector3.zero, Quaternion.identity);
            }
        }
        else
        {
            PhotonNetwork.Instantiate(PlayerLobby.name, Vector3.zero, Quaternion.identity);
        }
    }

    private void Update()
    {
        if(cam)
        if (!cam.enabled)
        {
            Entity[] entitys = FindObjectsByType<Entity>(FindObjectsSortMode.None);
            if (entitys.Length > 1)
            {
                cam.enabled = true;
            }
        }
    }


    /////////////////////네트워크////////////////////////
    public override void OnJoinedRoom()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        base.OnJoinedRoom();
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        PhotonNetwork.CreateRoom("Fight", new RoomOptions { MaxPlayers = 2 });
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.JoinRoom("Fight");
    }
    /////////////////////////////////////////////////////
}
