using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Animator))]
    public class ErrorDisplayer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI errorText;
        [SerializeField] private Button[] hideButtons;

        private Animator _animator;
        private static readonly int Show = Animator.StringToHash("show");
        private static readonly int Hide = Animator.StringToHash("hide");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
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
            _animator.SetTrigger(Show);
        }

        private void HideError()
        {
            _animator.SetTrigger(Hide);
        }
    }
}
