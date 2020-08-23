using System;
using Photon.Pun;
using Photon.Realtime;
using UI;
using UI.Screens;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Random = UnityEngine.Random;
using Screen = UI.Screens.Screen;

namespace Photon
{
    public class PhotonLobby : MonoBehaviourPunCallbacks
    {
        [SerializeField] private MultiplayerSettings settings;
        [SerializeField] private ScreensController screensController;
        [SerializeField] private ErrorDisplayer errorDisplayer;

        public event Action OnConnectToMaster;

        private void Start()
        {
            PhotonNetwork.ConnectUsingSettings();
            if (PhotonNetwork.IsConnected) OnConnectToMaster?.Invoke();
        }

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            OnConnectToMaster?.Invoke();
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            base.OnJoinRandomFailed(returnCode, message);
            Loader.Instance.StopLoading();
            errorDisplayer.ShowError(message);
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.LogError(message);
            CreateRoom();
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
            screensController.ShowScreen(Screen.WaitingScreen);
        }

        public void ChangeNickname(string nickName)
        {
            PhotonNetwork.NickName = nickName;
            screensController.ShowScreen(Screen.WaitingScreen);
        }

        public void JoinRoom(string roomName)
        {
            Loader.Instance.StartLoading();
            var result = PhotonNetwork.JoinRoom(roomName.ToUpper());
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

        public override void OnDisconnected(DisconnectCause cause)
        {
            errorDisplayer.ShowError("Error connecting to the servers. Try again later");
        }
    }
}
