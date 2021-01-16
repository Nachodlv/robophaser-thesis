using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Utils;
using Utils.Pools;

namespace Audio
{
    public enum AudioType
    {
        Reload,
        Button,
        ReceiveShootAsPlayer,
        ReceiveShoot,
        WallPhase,
        Shoot
    }

    [Serializable]
    public class AudioClipWithAudioType : IDictionaryEntity<AudioType, AudioClip>
    {
        public AudioType audioType;
        public AudioClip audioClip;
        public AudioType TValue => audioType;
        public AudioClip TsValue => audioClip;
    }

    [Serializable]
    public class AudioSettings
    {
        public AudioType audioType;
        public float volume = 1;
        public bool replicated;
        public bool spatial;
    }

    [RequireComponent(typeof(AudioSource))]
    public class AudioManager: PunSingleton<AudioManager>
    {
        [SerializeField] private int audioSourceQuantity;
        [SerializeField] private AudioSourcePooleable audioSourcePrefab;
        [SerializeField] private AudioClipWithAudioType[] audioClipWithAudioType;

        private AudioSource _audioSource;
        private ObjectPooler<AudioSourcePooleable> _pooler;
        private Dictionary<AudioType, AudioClip> _audios;

        protected override void Awake()
        {
            base.Awake();
            _audioSource = GetComponent<AudioSource>();
            _pooler = new ObjectPooler<AudioSourcePooleable>();
            _audios = ArrayToDictionary.ToDictionary<AudioType, AudioClip, AudioClipWithAudioType>(audioClipWithAudioType);
            _pooler.InstantiateObjects(audioSourceQuantity, audioSourcePrefab, "Audio Sources");
        }

        public AudioClip GetAudioClip(AudioType type) => _audios[type];

        public void PlaySoundOnPosition(AudioSettings settings, Vector3 position)
        {
            var settingsBytes = ByteArray.ObjectToByteArray(settings);
            if(settings.replicated) photonView.RPC(nameof(RPC_PlaySoundOnPosition), RpcTarget.All, settingsBytes, position);
            else RPC_PlaySoundOnPosition(settingsBytes, position);
        }

        public void PlaySound(AudioSettings settings)
        {
            var settingsBytes = ByteArray.ObjectToByteArray(settings);
           if(settings.replicated) photonView.RPC(nameof(RPC_PlaySound), RpcTarget.All, settingsBytes);
           else RPC_PlaySound(settingsBytes);
        }

        [PunRPC]
        private void RPC_PlaySoundOnPosition(byte[] settings, Vector3 position)
        {
            var audioSource = _pooler.GetNextObject();
            audioSource.Spatialize = true;
            audioSource.Transform.position = position;
            SetAudioSourceSettings(audioSource, ByteArray.ByteArrayToObject<AudioSettings>(settings));
        }

        [PunRPC]
        private void RPC_PlaySound(byte[] settings)
        {
            var audioSource = _pooler.GetNextObject();
            audioSource.Spatialize = false;
            SetAudioSourceSettings(audioSource, ByteArray.ByteArrayToObject<AudioSettings>(settings));
        }

        private void SetAudioSourceSettings(AudioSourcePooleable audioSource, AudioSettings settings)
        {
            audioSource.SetClip(settings.audioType);
            audioSource.SetVolume(settings.volume);
            audioSource.StartClip();
        }
    }
}
