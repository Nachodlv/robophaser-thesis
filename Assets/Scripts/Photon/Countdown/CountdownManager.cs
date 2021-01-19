using Cues;
using Photon.Pun;
using UnityEngine;
using Utils;

namespace Photon.Countdown
{
    public class CountdownManager : PunSingleton<CountdownManager>
    {
        [SerializeField] private Cue countdownCue;
        [SerializeField] private float countdownCueTime;
        [SerializeField] private StartCountdownEvent startCountdownEvent;
        [SerializeField] private FinishCountdownEvent finishCountdownEvent;

        private WaitSeconds _waitSeconds;

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
            startCountdownEvent.TriggerEvent(time);
        }

        [PunRPC]
        private void RPC_BroadcastFinishCountdown()
        {
            finishCountdownEvent.TriggerEvent();
        }
    }
}
