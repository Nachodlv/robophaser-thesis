using System;
using Photon;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screens
{
    public class TitleScreen : MonoBehaviour
    {
        [SerializeField] private TMP_InputField nicknameInput;
        [SerializeField] private Button playButton;
        [SerializeField] private PhotonLobby photonLobby;
        [SerializeField] private ScreensController screensController;

        private bool _connectedToMaster;

        private void Awake()
        {
            playButton.interactable = false;
            nicknameInput.onValueChanged.AddListener(text => playButton.interactable = text.Length > 0);
            nicknameInput.onSubmit.AddListener(text => InputSubmitted());
            playButton.onClick.AddListener(InputSubmitted);
            photonLobby.OnConnectToMaster += ConnectedToMaster;
        }

        private void Start()
        {
            if (!_connectedToMaster) Loader.Instance.StartLoading();
            if (!string.IsNullOrEmpty(PhotonNetwork.NickName))
            {
                screensController.ShowScreen(Screen.WaitingScreen);
            }
        }

        private void InputSubmitted()
        {
            var text = nicknameInput.text;
            if (text.Length == 0) return;
            photonLobby.ChangeNickname(text);
            photonLobby.OnConnectToMaster -= ConnectedToMaster;
        }

        private void ConnectedToMaster()
        {
            Loader.Instance.StopLoading();
            _connectedToMaster = true;
        }
    }
}
