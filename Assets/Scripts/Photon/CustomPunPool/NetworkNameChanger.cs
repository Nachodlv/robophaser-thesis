using Photon.Pun;

namespace Photon.CustomPunPool
{
    public class NetworkNameChanger : MonoBehaviourPun
    {
        public void ChangeName(string newName)
        {
            photonView.RPC(nameof(RPC_ChangeName), RpcTarget.AllBuffered, newName);
        }

        [PunRPC]
        private void RPC_ChangeName(string newName)
        {
            gameObject.name = newName;
        }
    }
}
