using System;
using System.Collections;
using UnityEngine;
using Utils.Pools;

namespace Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourcePooleable: Pooleable
    {
        private AudioSource _audioSource;
        private Func<IEnumerator> _waitClipToStopCached;
        public Transform Transform { get; private set; }

        public bool Spatialize
        {
            set => _audioSource.spatialize = value;
        }

        public bool Loop
        {
            get => _audioSource.loop;
            set => _audioSource.loop = value;
        }

        public float TimeBetweenLoops { get; set; }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.loop = false;
            _waitClipToStopCached = WaitClipToStop;
            Transform = transform;
        }

        public void SetClip(AudioType clip)
        {
            _audioSource.clip = AudioManager.Instance.GetAudioClip(clip);
        }

        public void SetVolume(float volume)
        {
            _audioSource.volume = volume;
        }

        public void StartClip()
        {
            _audioSource.Play();
            StartCoroutine(_waitClipToStopCached());
        }

        private IEnumerator WaitClipToStop()
        {
            var shouldLoop = true;
            while (shouldLoop)
            {
                shouldLoop = Loop;
                var clipLength = _audioSource.clip.length;
                var now = Time.time;
                while (Time.time - now < clipLength)
                    yield return null;

                if (!Loop) continue;
                var finished = Time.time;
                while (Time.time - finished < TimeBetweenLoops)
                    yield return null;
            }
            Deactivate();
        }
    }
}
