using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class NicknameDisplayer : MonoBehaviour
    {
        private TextMeshProUGUI _textComponent;

        private void Awake()
        {
            _textComponent = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            _textComponent.text = PhotonNetwork.NickName;
        }
    }
}
