using Photon.CustomPunPool;
using Photon.Pun;
using UnityEngine;

namespace Audio
{
    public class AudioSourceNetworkPooleable : AudioSourcePooleable
    {
        [SerializeField] private PunPooleable punPooleable;
        [SerializeField] private PhotonView photonView;

        public override void SetClip(AudioType clip)
        {
            base.SetClip(clip);
            photonView.RPC(nameof(RPC_SetClip), RpcTarget.Others, clip);
        }

        public override void SetVolume(float volume)
        {
            base.SetVolume(volume);
            photonView.RPC(nameof(RPC_SetVolume), RpcTarget.Others, volume);
        }

        public override void StartClip()
        {
            base.StartClip();
            photonView.RPC(nameof(RPC_StartClip), RpcTarget.Others);
        }

        public override void Deactivate()
        {
            punPooleable.IsActive = false;
            base.Deactivate();
        }

        [PunRPC]
        private void RPC_StartClip()
        {
            AudioSource.Play();
        }

        [PunRPC]
        private void RPC_SetVolume(float volume)
        {
            AudioSource.volume = volume;
        }

        [PunRPC]
        private void RPC_SetClip(AudioType clip)
        {
            AudioSource.clip = AudioManager.Instance.GetAudioClip(clip);
        }
    }
}
