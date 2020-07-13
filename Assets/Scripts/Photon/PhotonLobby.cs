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
        [SerializeField] private MultiplayerSettings settings;
        
        private void Awake()
        {
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
            PhotonNetwork.AutomaticallySyncScene = true;
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

        private void CreateRoom()
        {
            var randomRoomNumber = Random.Range(0, 1000); //TODO probably can be improved
            var roomOptions = new RoomOptions{IsVisible = true, IsOpen = true, MaxPlayers = (byte) settings.maxPlayers};
            PhotonNetwork.CreateRoom($"Room{randomRoomNumber}", roomOptions);
        }
      
    }
}