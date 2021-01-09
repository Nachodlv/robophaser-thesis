using System;
using Photon;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Modal))]
    public class WaitRematchResponseModal : MonoBehaviour
    {
        [SerializeField] private GameObject waitingForResponseText;
        [SerializeField] private GameObject declinedText;
        [SerializeField] private Button cancelRematchButton;
        [SerializeField] private GameObject spinner;

        private Modal _modal;
        private Modal Modal => _modal != null ? _modal : _modal = GetComponent<Modal>();

        private void Awake()
        {
            RematchManager.Instance.OnAcceptRematch += () => Modal.HideModal();
            RematchManager.Instance.OnDeclineRematch += () => WaitingForResponse(false);
            RematchManager.Instance.OnCancelRematch += () => Modal.HideModal();
        }

        public void Cancel()
        {
            RematchManager.Instance.CancelRematch();
            Modal.HideModal();
        }

        public void ShowModal()
        {
            WaitingForResponse(true);
            Modal.ShowModal();
        }

        private void WaitingForResponse(bool waiting)
        {
            waitingForResponseText.SetActive(waiting);
            declinedText.SetActive(!waiting);
            spinner.SetActive(waiting);
            cancelRematchButton.interactable = waiting;
        }
    }
}
