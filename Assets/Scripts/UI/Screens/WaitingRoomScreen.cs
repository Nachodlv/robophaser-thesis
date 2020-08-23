using Photon;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screens
{
    public class WaitingRoomScreen : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Button joinGameButton;
        [SerializeField] private Button createGameButton;
        [SerializeField] private Button goBackButton;
        [SerializeField] private PhotonLobby photonLobby;
        [SerializeField] private ScreensController screensController;

        private void Awake()
        {
            createGameButton.onClick.AddListener(photonLobby.CreateRoom);
            joinGameButton.interactable = false;
            joinGameButton.onClick.AddListener(() => photonLobby.JoinRoom(inputField.text));
            goBackButton.onClick.AddListener(() => screensController.ShowScreen(Screen.TitleScreen));
            inputField.onValueChanged.AddListener(InputFieldChanged);
            inputField.onSubmit.AddListener(photonLobby.JoinRoom);
        }

        private void InputFieldChanged(string text)
        {
            joinGameButton.interactable = text.Length > 0;
        }



    }
}
