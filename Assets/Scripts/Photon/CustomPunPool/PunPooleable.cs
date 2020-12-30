using Photon.Pun;
using UnityEngine;

namespace Photon.CustomPunPool
{
    public class PunPooleable : MonoBehaviourPun
    {
        private bool _isActive;

        public bool IsActive
        {
            get => _isActive;
            set => photonView.RPC(nameof(RPC_SetIsActive), RpcTarget.All, value);
        }

        public void SetParent(int viewId)
        {
            photonView.RPC(nameof(RPC_SetParent), RpcTarget.All, viewId);
        }

        public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            photonView.RPC(nameof(RPC_SetPositionAndRotation), RpcTarget.All, position, rotation);
        }

        [PunRPC]
        private void RPC_SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            transform.SetPositionAndRotation(position, rotation);
        }

        [PunRPC]
        private void RPC_SetIsActive(bool isActive)
        {
            _isActive = isActive;
            gameObject.SetActive(isActive);
        }

        [PunRPC]
        private void RPC_SetParent(int viewId)
        {
            var parent = PhotonNetwork.GetPhotonView(viewId).transform;
            if (parent != null)
            {
                transform.SetParent(parent);
            }
        }
    }
}
