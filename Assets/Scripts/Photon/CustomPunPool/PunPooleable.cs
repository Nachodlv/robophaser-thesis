using System;
using Photon.Pun;
using UnityEngine;

namespace Photon.CustomPunPool
{
    public class PunPooleable : MonoBehaviourPun
    {

        public bool IsActive
        {
            get => gameObject.activeSelf;
            set => photonView.RPC(nameof(RPC_SetIsActive), RpcTarget.All, value);
        }

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public void SetParent(int viewId)
        {
            photonView.RPC(nameof(RPC_SetParent), RpcTarget.All, viewId);
        }

        public void SetPositionAndRotationAndActivate(Vector3 position, Quaternion rotation)
        {
            photonView.RPC(nameof(RPC_SetPositionAndRotationAndActivate), RpcTarget.All, position, rotation);
        }

        [PunRPC]
        private void RPC_SetPositionAndRotationAndActivate(Vector3 position, Quaternion rotation)
        {
            if(!gameObject.activeSelf) gameObject.SetActive(true);
            transform.SetPositionAndRotation(position, rotation);
        }

        [PunRPC]
        private void RPC_SetIsActive(bool isActive)
        {
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
