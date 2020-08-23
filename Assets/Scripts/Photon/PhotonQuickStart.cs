using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Photon
{
    public class PhotonQuickStart : MonoBehaviourPunCallbacks
    {
        [SerializeField] private MultiplayerSettings settings;
        private void Awake()
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            var roomOptions = new RoomOptions
                {IsVisible = true, IsOpen = true, MaxPlayers = (byte) settings.maxPlayers};
            PhotonNetwork.CreateRoom("", roomOptions);
        }
    }
}
