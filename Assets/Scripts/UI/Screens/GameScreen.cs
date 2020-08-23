using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screens
{
    public class GameScreen : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TextMeshProUGUI codeText;
        [SerializeField] private Button leaveGameButton;
        [SerializeField] private ScreensController screensController;

        private void Awake()
        {
            codeText.text = "";
            leaveGameButton.onClick.AddListener(LeaveRoom);
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

        private void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom(false);
            screensController.ShowScreen(Screen.WaitingScreen);
        }
    }
}
