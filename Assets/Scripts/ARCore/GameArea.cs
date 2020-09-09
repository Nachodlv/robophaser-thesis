using System;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace ARCore
{
    public class GameArea : MonoBehaviour
    {
        [SerializeField] private Slider xSlider;
        [SerializeField] private Slider zSlider;
        [SerializeField] private Button confirmButton;
        [SerializeField] private GameObject mesh;
        [SerializeField] private Modal slidersMenu;
        public delegate void OnConfirmCallback(float width, float depth);
        public event OnConfirmCallback OnConfirmChanges;

        public Vector3 GameAreaPosition => mesh.transform.position;
        public Quaternion GameAreaRotation => mesh.transform.rotation;

        private void Awake()
        {
            var localScale = mesh.transform.localScale;
            xSlider.value = localScale.x;
            zSlider.value = localScale.z;

            xSlider.onValueChanged.AddListener(ScaleInX);
            zSlider.onValueChanged.AddListener(ScaleInZ);

            confirmButton.onClick.AddListener(ConfirmChanges);

            mesh.SetActive(false);
        }

        public void ShowGameArea(Vector3 position, Quaternion rotation)
        {
            mesh.SetActive(true);
            mesh.transform.position = position;
            mesh.transform.rotation = rotation;
            slidersMenu.ShowModal();
        }

        private void ScaleInX(float scale)
        {
            var transform1 = mesh.transform;
            var transformLocalScale = transform1.localScale;
            transformLocalScale.x = scale;
            transform1.localScale = transformLocalScale;
        }

        private void ScaleInZ(float scale)
        {
            var transform1 = mesh.transform;
            var transformLocalScale = transform1.localScale;
            transformLocalScale.z = scale;
            transform1.localScale = transformLocalScale;
        }

        private void ConfirmChanges()
        {
            var localScale = mesh.transform.localScale;
            OnConfirmChanges?.Invoke(localScale.x, localScale.z);
            mesh.SetActive(false);
            slidersMenu.HideModal();
        }
    }
}
