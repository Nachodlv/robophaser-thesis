using System;
using ARCore;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Modal))]
    public class SlidersMenu : MonoBehaviour
    {
        [SerializeField] private Slider xSlider;
        [SerializeField] private Slider zSlider;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button changePositionButton;

        public event Action OnConfirmChanges;
        public event Action<float> OnXSliderValueChange;
        public event Action<float> OnZSliderValueChange;
        public event Action OnChangePosition;

        private Modal _modal;

        private void Awake()
        {
            _modal = GetComponent<Modal>();
            confirmButton.onClick.AddListener(() => OnConfirmChanges?.Invoke());
            changePositionButton.onClick.AddListener(() => OnChangePosition?.Invoke());
            xSlider.onValueChanged.AddListener(value => OnXSliderValueChange?.Invoke(value));
            zSlider.onValueChanged.AddListener(value => OnZSliderValueChange?.Invoke(value));
        }

        public void ShowModal() => _modal.ShowModal();
        public void HideModal() => _modal.HideModal();

        public float XSliderValue
        {
            set => xSlider.value = value;
        }

        public float ZSliderValue
        {
            set => zSlider.value = value;
        }
    }
}
