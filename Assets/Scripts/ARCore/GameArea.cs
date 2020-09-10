using Photon.Pun;
using UI;
using UnityEngine;

namespace ARCore
{
    public class GameArea : MonoBehaviourPun
    {
        [SerializeField] private GameObject mesh;

        public delegate void OnConfirmCallback(float width, float depth);
        public event OnConfirmCallback OnConfirmChanges;

        public Vector3 GameAreaPosition => mesh.transform.position;
        public Quaternion GameAreaRotation => mesh.transform.rotation;

        private SlidersMenu _slidersMenu;

        private void Awake()
        {
            _slidersMenu = FindObjectOfType<SlidersMenu>();

            var localScale = mesh.transform.localScale;
            _slidersMenu.XSliderValue = localScale.x;
            _slidersMenu.ZSliderValue = localScale.z;

            _slidersMenu.OnXSliderValueChange += scale => photonView.RPC(nameof(RPC_ScaleInX), RpcTarget.All, scale);
            _slidersMenu.OnZSliderValueChange += scale => photonView.RPC(nameof(RPC_ScaleInZ), RpcTarget.All, scale);

            _slidersMenu.OnConfirmChanges += ConfirmChanges;

            mesh.SetActive(false);
        }

        public void ShowGameArea(Vector3 position, Quaternion rotation)
        {
            _slidersMenu.ShowModal();
            photonView.RPC(nameof(RPC_SetPositionAndRotation), RpcTarget.All, position, rotation);
        }

        private void ConfirmChanges()
        {
            var localScale = mesh.transform.localScale;
            OnConfirmChanges?.Invoke(localScale.x, localScale.z);
            _slidersMenu.HideModal();
            photonView.RPC(nameof(RPC_HideMesh), RpcTarget.All);
        }

        [PunRPC]
        private void RPC_SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            mesh.SetActive(true);
            mesh.transform.position = position;
            mesh.transform.rotation = rotation;
        }

        [PunRPC]
        private void RPC_ScaleInX(float scale)
        {
            var transform1 = mesh.transform;
            var transformLocalScale = transform1.localScale;
            transformLocalScale.x = scale;
            transform1.localScale = transformLocalScale;
        }

        [PunRPC]
        private void RPC_ScaleInZ(float scale)
        {
            var transform1 = mesh.transform;
            var transformLocalScale = transform1.localScale;
            transformLocalScale.z = scale;
            transform1.localScale = transformLocalScale;
        }

        [PunRPC]
        private void RPC_HideMesh()
        {
            mesh.SetActive(false);
        }
    }
}
