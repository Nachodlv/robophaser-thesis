using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [Serializable]
    public class FlashSettings
    {
        public float speed = 0.4f;
        public float maxAlpha = 0.15f;
        public float minAlpha = 0;
        public int quantityOfFlashes = 1;
        public Color color = Color.red;
        public bool loop;
        public float timeBetweenFlashes;
    }

    public class ImageFlash : MonoBehaviour
    {
        [SerializeField] private Image image;

        private Func<FlashSettings, IEnumerator> _startFlashingCached;
        private Func<FlashSettings, float, IEnumerator> _flashLoopingCached;
        private Func<bool, FlashSettings, IEnumerator> _changeFlashVisibilityCached;
        private bool _flashing;
        private bool _changingVisibility;
        private Coroutine _startFlashingCoroutine;
        private Coroutine _changeVisibilityCoroutine;
        private Dictionary<float, Coroutine> _loopingFlashes = new Dictionary<float, Coroutine>();

        private void Awake()
        {
            _startFlashingCached = StartFlashing;
            _flashLoopingCached = FlashLooping;
            _changeFlashVisibilityCached = ChangeFlashVisibility;
        }

        public void Flash(FlashSettings flashSettings)
        {
            image.color = flashSettings.color;
            if(_flashing && _startFlashingCoroutine != null) StopCoroutine(_startFlashingCoroutine);
            if(_changingVisibility && _changeVisibilityCoroutine != null) StopCoroutine(_changeVisibilityCoroutine);
            _startFlashingCoroutine = StartCoroutine(_startFlashingCached(flashSettings));
        }

        public float LoopFlash(FlashSettings settings)
        {
            var id = Time.time;
            settings.color.a = 0;
            image.color = settings.color;
            _loopingFlashes.Add(id, StartCoroutine(_flashLoopingCached(settings, id)));
            return id;
        }

        public void StopLoopingFlash(float id, FlashSettings settings)
        {
            if (_loopingFlashes.TryGetValue(id, out var flashCoroutine))
            {
                StopCoroutine(flashCoroutine);
                _changeVisibilityCoroutine = StartCoroutine(_changeFlashVisibilityCached(false, settings));
            }
        }

        private IEnumerator FlashLooping(FlashSettings flashSettings, float id)
        {
            while (true)
            {
                yield return _startFlashingCached(flashSettings);
                var now = Time.time;
                while (Time.time - now < flashSettings.timeBetweenFlashes) yield return null;
            }
        }

        private IEnumerator StartFlashing(FlashSettings flashSettings)
        {
            _flashing = true;
            var showing = true;
            var imageColor = image.color;
            imageColor.a = 0;
            image.color = imageColor;
            for (var i = 0; i < flashSettings.quantityOfFlashes * 2; i++)
            {
                yield return _changeFlashVisibilityCached(showing, flashSettings);
                showing = !showing;
            }
            _flashing = false;
        }

        private IEnumerator ChangeFlashVisibility(bool show, FlashSettings flashSettings)
        {
            _changingVisibility = true;
            while (true)
            {
                var color = image.color;
                if (show && color.a >= flashSettings.maxAlpha) break;
                if (!show && color.a <= flashSettings.minAlpha) break;
                var imageColor = color;
                imageColor.a += flashSettings.speed * Time.deltaTime * (show ? 1 : -1);
                image.color = imageColor;
                yield return null;
            }
            _changingVisibility = false;
        }
    }
}
