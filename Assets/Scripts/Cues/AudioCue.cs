using Audio;
using UnityEngine;
using AudioSettings = Audio.AudioSettings;

namespace Cues
{
    [CreateAssetMenu(fileName = "New Audio Cue", menuName = "Cue/Audio", order = 0)]
    public class AudioCue : Cue
    {
        [SerializeField] private AudioSettings settings;

        public override void Execute(Vector3 position, Quaternion rotation)
        {
            if(settings.spatial) AudioManager.Instance.PlaySoundOnPosition(settings, position);
            else AudioManager.Instance.PlaySound(settings);
        }
    }
}
