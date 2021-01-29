using System;
using Audio;
using UnityEngine;
using AudioSettings = Audio.AudioSettings;

namespace Cues
{
    [CreateAssetMenu(fileName = "New Audio Cue", menuName = "Cue/Audio", order = 0)]
    public class AudioCue : Cue
    {
        [SerializeField] internal AudioSettings settings;
        [NonSerialized] private float _audioLoopId;

        public override void Execute(Vector3 position, Quaternion rotation)
        {
            var audioManager = AudioManager.Instance;
            if (audioManager == null) return;
            if (settings.loop)
            {
                _audioLoopId = audioManager.PlayLoopingSound(settings);
            }
            else
            {
                if(settings.spatial) audioManager.PlaySoundOnPosition(settings, position);
                else audioManager.PlaySound(settings);
            }
        }

        public override void StopExecution()
        {
            var audioManager = AudioManager.Instance;
            if (audioManager == null) return;
            audioManager.StopLoopingSound(_audioLoopId, settings);
        }
    }
}
