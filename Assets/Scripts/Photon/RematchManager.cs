using System;
using Photon.Pun;
using Utils;

namespace Photon
{
    public class RematchManager : PunSingleton<RematchManager>
    {
        private bool _waitingForResponse;

        public delegate void RematchReceivedCallback(string requesterNickname);

        public event Action OnAcceptRematch;
        public event Action OnDeclineRematch;
        public event Action OnCancelRematch;
        public event RematchReceivedCallback OnRematchReceived;

        protected override void Awake()
        {
            base.Awake();
            PhotonRoom.Instance.OnOpponentDisconnect += CancelRematch;
        }

        public void Rematch()
        {
            _waitingForResponse = true;
            photonView.RPC(nameof(RPC_Rematch), RpcTarget.Others, PhotonNetwork.NickName);
        }

        public void CancelRematch()
        {
            if (_waitingForResponse)
            {
                photonView.RPC(nameof(RPC_RematchCanceled), RpcTarget.All);
            }
        }

        public void RematchAccepted()
        {
            if (_waitingForResponse)
            {
                photonView.RPC(nameof(RPC_RematchAccepted), RpcTarget.All);
            }
        }

        public void RematchDeclined()
        {
            if (_waitingForResponse)
            {
                photonView.RPC(nameof(RPC_RematchDeclined), RpcTarget.All);
            }
        }

        [PunRPC]
        private void RPC_Rematch(string nickname)
        {
            _waitingForResponse = true;
            OnRematchReceived?.Invoke(nickname);
        }

        [PunRPC]
        private void RPC_RematchAccepted()
        {
            _waitingForResponse = false;
            OnAcceptRematch?.Invoke();
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonRoom.Instance.RestartRoom();
            SceneLoader.Instance.LoadSceneAsync(PhotonRoom.Instance.Settings.multiplayerScene);
        }

        [PunRPC]
        private void RPC_RematchDeclined()
        {
            _waitingForResponse = false;
            OnDeclineRematch?.Invoke();
        }

        [PunRPC]
        private void RPC_RematchCanceled()
        {
            _waitingForResponse = false;
            OnCancelRematch?.Invoke();
        }
    }
}
