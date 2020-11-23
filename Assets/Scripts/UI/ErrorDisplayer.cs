using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI
{
    [RequireComponent(typeof(Modal))]
    public class ErrorDisplayer : Singleton<ErrorDisplayer>
    {
        [SerializeField] private TextMeshProUGUI errorText;
        [SerializeField] private Button[] hideButtons;

        private Modal _modal;

        private void Awake()
        {
            _modal = GetComponent<Modal>();
            foreach (var hideButton in hideButtons)
            {
                hideButton.onClick.AddListener(HideError);
            }
        }

        private void OnDestroy()
        {
            foreach (var hideButton in hideButtons)
            {
                hideButton.onClick.RemoveListener(HideError);
            }
        }

        public void ShowError(string error)
        {
            errorText.text = error;
            _modal.ShowModal();
        }

        private void HideError()
        {
            _modal.HideModal();
        }
    }
}
