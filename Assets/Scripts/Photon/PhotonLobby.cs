using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Photon
{
    public class PhotonLobby : MonoBehaviourPunCallbacks
    {
        [SerializeField] private Button battleButton;
        [SerializeField] private Button cancelButton;
        
        public static PhotonLobby Lobby;

        private void Awake()
        {
            Lobby = this;
            battleButton.interactable = false;
            battleButton.onClick.AddListener(OnBattleButtonClicked);
            cancelButton.onClick.AddListener(OnCancelButtonClicked);
        }

        private void Start()
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        private void OnDestroy()
        {
            battleButton.onClick.RemoveListener(OnBattleButtonClicked);
            cancelButton.onClick.RemoveListener(OnCancelButtonClicked);
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Player has connected to the Photon master server");
            battleButton.interactable = true;
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log($"Failed to join a random room with error: {message} and return code: {returnCode}");
            CreateRoom();
        }
        
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log($"Failed to create a room with error: {message} and return code: {returnCode}");
            CreateRoom();
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("Player joined a room!");
        }

        private void OnBattleButtonClicked()
        {
            battleButton.gameObject.SetActive(false);
            cancelButton.gameObject.SetActive(true);
            PhotonNetwork.JoinRandomRoom();
        }

        private void OnCancelButtonClicked()
        {
            cancelButton.gameObject.SetActive(false);
            battleButton.gameObject.SetActive(true);
            PhotonNetwork.LeaveRoom();
        }

        private static void CreateRoom()
        {
            var randomRoomNumber = Random.Range(0, 1000);
            var roomOptions = new RoomOptions{IsVisible = true, IsOpen = true, MaxPlayers = 2};
            PhotonNetwork.CreateRoom("Room" + randomRoomNumber, roomOptions);
        }
      
    }
}