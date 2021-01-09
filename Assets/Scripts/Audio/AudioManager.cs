using UnityEngine;
using Utils;
using Utils.Pools;

namespace Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager: Singleton<AudioManager>
    {
        [SerializeField] private int audioSourceQuantity;
        [SerializeField] private AudioSourcePooleable audioSourcePrefab;

        private AudioSource _audioSource;
        private ObjectPooler<AudioSourcePooleable> _pooler;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _pooler = new ObjectPooler<AudioSourcePooleable>();
            _pooler.InstantiateObjects(audioSourceQuantity, audioSourcePrefab, "Audio Sources");
        }

        public void PlaySoundOnPosition(AudioClip clip, Vector3 position, float volume = 1)
        {
            var audioSource = _pooler.GetNextObject();
            audioSource.Transform.position = position;
            audioSource.SetClip(clip);
            audioSource.SetVolume(volume);
            audioSource.StartClip();
        }
    }
}
