using System;
using Photon.Pun;
using Utils;

namespace Photon
{
    public class CountdownManager : PunSingleton<CountdownManager>
    {
        private WaitSeconds _waitSeconds;

        public delegate void StartCountdownCallback(float countdownTime);
        public event StartCountdownCallback OnStartCountdown;
        public event Action OnFinishCountdown;

        protected override void Awake()
        {
            base.Awake();
            _waitSeconds = new WaitSeconds(this, FinishCountdown);
        }

        public void StartCountdown(float time)
        {
            _waitSeconds.Wait(time);
            photonView.RPC(nameof(RPC_BroadcastStartCountdown), RpcTarget.All, time);
        }

        private void FinishCountdown()
        {
            photonView.RPC(nameof(RPC_BroadcastFinishCountdown), RpcTarget.All);
        }

        [PunRPC]
        private void RPC_BroadcastStartCountdown(float time)
        {
            OnStartCountdown?.Invoke(time);
        }

        [PunRPC]
        private void RPC_BroadcastFinishCountdown()
        {
            OnFinishCountdown?.Invoke();
        }

    }
}
