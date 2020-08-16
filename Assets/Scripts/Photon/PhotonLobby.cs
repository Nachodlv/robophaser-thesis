using System;
using Photon.Pun;
using Photon.Realtime;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Screen = UI.Screen;

namespace Photon
{
    public class PhotonLobby : MonoBehaviourPunCallbacks
    {
        [SerializeField] private MultiplayerSettings settings;
        [SerializeField] private ScreensController screensController;
        [SerializeField] private ErrorDisplayer errorDisplayer;

        private void Start()
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Player has connected to the Photon master server");
            PhotonNetwork.AutomaticallySyncScene = true;
            screensController.ShowScreen(Screen.WaitingScreen);
            errorDisplayer.ShowError("Player has connected to the Photon master server");
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

        public void JoinRandomRoom()
        {
            PhotonNetwork.JoinRandomRoom();
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        private void CreateRoom()
        {
            var randomRoomNumber = Random.Range(0, 1000); //TODO probably can be improved
            var roomOptions = new RoomOptions{IsVisible = true, IsOpen = true, MaxPlayers = (byte) settings.maxPlayers};
            PhotonNetwork.CreateRoom($"Room{randomRoomNumber}", roomOptions);
        }

        public override void OnErrorInfo(ErrorInfo errorInfo)
        {
            base.OnErrorInfo(errorInfo);
        }
    }
}
