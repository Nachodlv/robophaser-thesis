using Photon.Pun;
using UnityEngine;

namespace Utils.Pools
{
    [RequireComponent(typeof(PhotonView))]
    public class NetworkPooleable: Pooleable
    {
        private PhotonView _photonView;
        protected PhotonView PhotonView => _photonView != null ? _photonView : _photonView = GetComponent<PhotonView>();

        public override void Activate()
        {
            PhotonView.RPC(nameof(RPC_Activate), RpcTarget.All);
        }

        public override void Deactivate()
        {
            PhotonView.RPC(nameof(RPC_Deactivate), RpcTarget.All);
        }

        [PunRPC]
        private void RPC_Activate()
        {
            base.Activate();
        }

        [PunRPC]
        private void RPC_Deactivate()
        {
            base.Deactivate();
        }
    }
}
