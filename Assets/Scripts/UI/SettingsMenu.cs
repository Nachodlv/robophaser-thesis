using System;
using Photon;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SettingsMenu : MonoBehaviour
    {
        [SerializeField] private Button leaveGameButton;

        private void Awake()
        {
            leaveGameButton.onClick.AddListener(() => PhotonRoom.Instance.LeaveRoom());
        }
    }
}
