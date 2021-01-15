using System;
using System.Collections;
using UnityEngine;
using Utils.Pools;

namespace Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourcePooleable: Pooleable
    {
        protected AudioSource AudioSource;
        private Func<IEnumerator> _playingSoundCoroutine;
        public Transform Transform { get; private set; }

        public bool Spatialize
        {
            get => AudioSource.spatialize;
            set => AudioSource.spatialize = value;
        }

        private void Awake()
        {
            AudioSource = GetComponent<AudioSource>();
            AudioSource.playOnAwake = false;
            AudioSource.loop = false;
            _playingSoundCoroutine = WaitClipToStop;
            Transform = transform;
        }

        public virtual void SetClip(AudioType clip)
        {
            AudioSource.clip = AudioManager.Instance.GetAudioClip(clip);
        }

        public virtual void SetVolume(float volume)
        {
            AudioSource.volume = volume;
        }

        public virtual void StartClip()
        {
            AudioSource.Play();
            StartCoroutine(_playingSoundCoroutine());
        }

        private IEnumerator WaitClipToStop()
        {
            var clipLength = AudioSource.clip.length;
            var now = Time.time;
            while (Time.time - now < clipLength)
                yield return null;
            Deactivate();
        }
    }
}
