using System;
using Cues;
using Photon.Pun;
using UnityEngine;
using Utils;

namespace Photon
{
    public class CountdownManager : PunSingleton<CountdownManager>
    {
        [SerializeField] private Cue countdownCue;
        [SerializeField] private float countdownCueTime;

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
            if (time <= countdownCueTime)
            {
                ExecuteCueAndWaitToFinish(time);
            }
            else
            {
                _waitSeconds.Wait(time - countdownCueTime, () => ExecuteCueAndWaitToFinish(time - countdownCueTime));
            }

            photonView.RPC(nameof(RPC_BroadcastStartCountdown), RpcTarget.All, time);
        }

        private void ExecuteCueAndWaitToFinish(float time)
        {
            countdownCue.Execute(transform.position, Quaternion.identity);
            _waitSeconds.Wait(time);
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
