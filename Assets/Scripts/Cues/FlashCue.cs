using System;
using UI;
using UnityEngine;

namespace Cues
{
    [CreateAssetMenu(fileName = "New Flash Cue", menuName = "Cue/Flash", order = 0)]
    public class FlashCue : Cue
    {
        [SerializeField] private FlashSettings flashSettings;

        [NonSerialized] private ImageFlash _cameraFlash;

        private ImageFlash CameraFlash => _cameraFlash != null ? _cameraFlash : _cameraFlash = Camera.main?.GetComponent<ImageFlash>();

        public override void Execute(Vector3 position, Quaternion rotation)
        {
            CameraFlash.Flash(flashSettings);
        }
    }
}
