using System;
using Photon;
using TMPro;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(Modal))]
    public class RematchModal : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI message;
        [SerializeField] private string defaultMessage = "wants to play again!";
        [SerializeField] private Modal rematchCanceledModal;

        private Modal _modal;

        private Modal Modal => _modal != null ? _modal : _modal = GetComponent<Modal>();

        private void Awake()
        {
            RematchManager.Instance.OnRematchReceived += ShowModal;
            RematchManager.Instance.OnCancelRematch += RematchCanceled;
        }

        private void ShowModal(string nickname)
        {
            message.text = $"{nickname} {defaultMessage}";
            Modal.ShowModal();
        }

        public void Accept()
        {
            Modal.HideModal();
            RematchManager.Instance.RematchAccepted();
        }

        public void Decline()
        {
            Modal.HideModal();
            RematchManager.Instance.RematchDeclined();
        }

        private void RematchCanceled()
        {
            if (!Modal.Open) return;
            Modal.HideModal();
            rematchCanceledModal.ShowModal();
        }
    }
}
