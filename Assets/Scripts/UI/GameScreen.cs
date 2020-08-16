using System;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameScreen : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TextMeshProUGUI codeText;
        [SerializeField] private Button leaveGameButton;

        private void Awake()
        {
            codeText.text = "";
            leaveGameButton.onClick.AddListener(() => PhotonNetwork.LeaveRoom());
        }

        public override void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        public override void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            codeText.text = PhotonNetwork.CurrentRoom.Name;
        }
    }
}
