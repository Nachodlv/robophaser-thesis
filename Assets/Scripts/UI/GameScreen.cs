using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace UI
{
    public class GameScreen : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TextMeshProUGUI codeText;

        private void Awake()
        {
            codeText.text = "";
        }

        public override void OnJoinedRoom()
        {
            codeText.text = PhotonNetwork.CurrentRoom.Name;
        }
    }
}
