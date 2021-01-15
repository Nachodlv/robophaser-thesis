using System;
using System.Collections.Generic;
using Photon.CustomPunPool;
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
    public class AudioManager: Singleton<AudioManager>
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
            var audioSource = InstantiateAudioSource(settings, position);
            audioSource.Spatialize = true;
            SetAudioSourceSettings(audioSource, settings);
        }

        public void PlaySound(AudioSettings settings)
        {
            var audioSource = InstantiateAudioSource(settings, Vector3.zero);
            audioSource.Spatialize = false;
            SetAudioSourceSettings(audioSource, settings);
        }

        private void SetAudioSourceSettings(AudioSourcePooleable audioSource, AudioSettings settings)
        {
            audioSource.SetClip(settings.audioType);
            audioSource.SetVolume(settings.volume);
            audioSource.StartClip();
        }

        private AudioSourcePooleable InstantiateAudioSource(AudioSettings settings, Vector3 position)
        {
            if (settings.replicated)
            {
                return PunPool.Instance.Instantiate("Audio/Audio Source Network Pooleable", position, Quaternion.identity)
                    .GetComponent<AudioSourcePooleable>();
            }
            var pooleable = _pooler.GetNextObject();
            pooleable.Transform.position = position;
            return pooleable;
        }
    }
}
