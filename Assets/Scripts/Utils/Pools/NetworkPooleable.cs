using Photon.Pun;
using UnityEngine;

namespace Utils.Pools
{
    [RequireComponent(typeof(PhotonView))]
    public class NetworkPooleable: Pooleable
    {
        private PhotonView _photonView;

        protected PhotonView PhotonView => _photonView;

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
        }

        public override void Activate()
        {
            _photonView.RPC(nameof(RPC_Activate), RpcTarget.Others);
            base.Activate();
        }

        public override void Deactivate()
        {
            _photonView.RPC(nameof(RPC_Deactivate), RpcTarget.Others);
            base.Deactivate();
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
