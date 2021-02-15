using System;
using System.Collections.Generic;
using Cues;
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
        Shoot,
        Countdown,
        BackgroundMusic
    }

    [Serializable]
    public class AudioClipWithAudioType : IDictionaryEntity<AudioType, AudioClip>
    {
        public AudioType audioType;
        public AudioClip audioClip;
        public AudioType TValue => audioType;
        public AudioClip TsValue => audioClip;
    }

    [RequireComponent(typeof(AudioSource))]
    public class AudioManager: PunSingleton<AudioManager>
    {
        [SerializeField] private int audioSourceQuantity;
        [SerializeField] private AudioSourcePooleable audioSourcePrefab;
        [SerializeField] private AudioClipWithAudioType[] audioClipWithAudioType;
        [SerializeField] private AudioCue backgroundMusic;

        private ObjectPooler<AudioSourcePooleable> _pooler;
        private Dictionary<AudioType, AudioClip> _audios;
        private Dictionary<float, AudioSourcePooleable> _audioSourceLooping = new Dictionary<float, AudioSourcePooleable>();

        protected override void Awake()
        {
            base.Awake();
            _pooler = new ObjectPooler<AudioSourcePooleable>();
            _audios = ArrayToDictionary.ToDictionary<AudioType, AudioClip, AudioClipWithAudioType>(audioClipWithAudioType);
            _pooler.InstantiateObjects(audioSourceQuantity, audioSourcePrefab, "Audio Sources");
        }

        private void OnEnable()
        {
            PlayLoopingSound(backgroundMusic.settings);
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

        public float PlayLoopingSound(AudioSettings settings)
        {
            var id = Time.time;
            var settingsBytes = ByteArray.ObjectToByteArray(settings);
            if (settings.replicated) photonView.RPC(nameof(RPC_PlayLoopingSound), RpcTarget.All, settingsBytes, id);
            else RPC_PlayLoopingSound(settingsBytes, id);
            return id;
        }

        public void StopLoopingSound(float id, AudioSettings settings)
        {
            if(settings.replicated) photonView.RPC(nameof(RPC_StopPlayingLoopingSound), RpcTarget.All, id);
            else RPC_StopPlayingLoopingSound(id);
        }

        [PunRPC]
        private AudioSourcePooleable RPC_PlaySoundOnPosition(byte[] settings, Vector3 position)
        {
            var audioSource = _pooler.GetNextObject();
            audioSource.Spatialize = true;
            audioSource.Transform.position = position;
            SetAudioSourceSettings(audioSource, ByteArray.ByteArrayToObject<AudioSettings>(settings));
            return audioSource;
        }

        [PunRPC]
        private AudioSourcePooleable RPC_PlaySound(byte[] settings)
        {
            var audioSource = _pooler.GetNextObject();
            audioSource.Spatialize = false;
            SetAudioSourceSettings(audioSource, ByteArray.ByteArrayToObject<AudioSettings>(settings));
            return audioSource;
        }

        [PunRPC]
        private void RPC_PlayLoopingSound(byte[] settings, float id)
        {
            var audioSource = RPC_PlaySound(settings);
            _audioSourceLooping.Add(id, audioSource);
        }

        [PunRPC]
        private void RPC_StopPlayingLoopingSound(float id)
        {
            if (_audioSourceLooping.TryGetValue(id, out var audioSource))
            {
                audioSource.Deactivate();
            }
        }

        private void SetAudioSourceSettings(AudioSourcePooleable audioSource, AudioSettings settings)
        {
            audioSource.SetClip(settings.audioType);
            audioSource.SetVolume(settings.volume);
            audioSource.Loop = settings.loop;
            audioSource.TimeBetweenLoops = settings.loopSettings.timeBetweenLoops;
            audioSource.StartClip();
        }
    }
}
