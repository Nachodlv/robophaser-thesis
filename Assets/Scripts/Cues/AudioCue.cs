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
            if (settings.loop)
            {
                _audioLoopId = AudioManager.Instance.PlayLoopingSound(settings);
            }
            else
            {
                if(settings.spatial) AudioManager.Instance.PlaySoundOnPosition(settings, position);
                else AudioManager.Instance.PlaySound(settings);
            }
        }

        public override void StopExecution()
        {
            AudioManager.Instance.StopLoopingSound(_audioLoopId, settings);
        }
    }
}
