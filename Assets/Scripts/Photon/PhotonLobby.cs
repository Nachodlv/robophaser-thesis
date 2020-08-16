using System;
using Photon.Pun;
using Photon.Realtime;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Utils;
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
            PhotonNetwork.AutomaticallySyncScene = true;
            screensController.ShowScreen(Screen.WaitingScreen);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            errorDisplayer.ShowError(message);
            CreateRoom();
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            errorDisplayer.ShowError(message);
            CreateRoom();
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        public void JoinRoom(string roomName)
        {
            Loader.Instance.StartLoading();
            Debug.Log($"Trying to join room {roomName}");
            var result = PhotonNetwork.JoinRoom(roomName);
            if (result) return;
            Loader.Instance.StopLoading();
            errorDisplayer.ShowError("Error joining room. Try again later");
        }

        public void CreateRoom()
        {
            var roomOptions = new RoomOptions{IsVisible = true, IsOpen = true, MaxPlayers = (byte) settings.maxPlayers};
            Loader.Instance.StartLoading();
            screensController.ShowScreen(Screen.GameScreen);
            PhotonNetwork.CreateRoom(RandomString.CreateString(5), roomOptions);
        }

        public override void OnErrorInfo(ErrorInfo errorInfo)
        {
            base.OnErrorInfo(errorInfo);
            errorDisplayer.ShowError(errorInfo.Info);
        }
    }
}
