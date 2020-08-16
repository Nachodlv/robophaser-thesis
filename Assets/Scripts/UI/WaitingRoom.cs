using System;
using Photon;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class WaitingRoom : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Button joinGameButton;
        [SerializeField] private Button createGameButton;
        [SerializeField] private PhotonLobby photonLobby;

        private void Awake()
        {
            createGameButton.onClick.AddListener(photonLobby.CreateRoom);
            joinGameButton.interactable = false;
            joinGameButton.onClick.AddListener(() => photonLobby.JoinRoom(inputField.text));
            inputField.onValueChanged.AddListener(InputFieldChanged);
            inputField.onEndEdit.AddListener(photonLobby.JoinRoom);
        }

        private void InputFieldChanged(string text)
        {
            joinGameButton.interactable = text.Length > 0;
        }



    }
}
