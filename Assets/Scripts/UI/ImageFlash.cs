using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [Serializable]
    public class FlashSettings
    {
        [SerializeField] private float speed = 0.4f;
        [SerializeField] private float maxAlpha = 0.15f;
        [SerializeField] private float minAlpha = 0;
        [SerializeField] private int quantityOfFlashes = 1;
        [SerializeField] private Color color = Color.red;

        public float Speed => speed;
        public float MaxAlpha => maxAlpha;
        public float MinAlpha => minAlpha;
        public int QuantityOfFlashes => quantityOfFlashes;

        public Color Color => color;

        public FlashSettings(){}

        public FlashSettings(float speed, float maxAlpha, float minAlpha, int quantityOfFlashes, Color color)
        {
            this.speed = speed;
            this.maxAlpha = maxAlpha;
            this.minAlpha = minAlpha;
            this.quantityOfFlashes = quantityOfFlashes;
            this.color = color;
        }

    }
    public class ImageFlash : MonoBehaviour
    {
        [SerializeField] private Image image;

        private Func<FlashSettings, IEnumerator> _startFlashingCached;
        private Func<bool, FlashSettings, IEnumerator> _changeFlashVisibilityCached;
        private bool _flashing;
        private Coroutine _startFlashingCoroutine;

        private void Awake()
        {
            _startFlashingCached = StartFlashing;
            _changeFlashVisibilityCached = ChangeFlashVisibility;
        }

        public void Flash(FlashSettings flashSettings)
        {
            image.color = flashSettings.Color;
            if(_flashing && _startFlashingCoroutine != null) StopCoroutine(_startFlashingCoroutine);
            _startFlashingCoroutine = StartCoroutine(_startFlashingCached(flashSettings));
        }

        private IEnumerator StartFlashing(FlashSettings flashSettings)
        {
            _flashing = true;
            var showing = true;
            var imageColor = image.color;
            imageColor.a = 0;
            image.color = imageColor;
            for (var i = 0; i < flashSettings.QuantityOfFlashes * 2; i++)
            {
                yield return _changeFlashVisibilityCached(showing, flashSettings);
                showing = !showing;
            }
            _flashing = false;
        }

        private IEnumerator ChangeFlashVisibility(bool show, FlashSettings flashSettings)
        {
            while (true)
            {
                var color = image.color;
                if (show && color.a >= flashSettings.MaxAlpha) yield break;
                if (!show && color.a <= flashSettings.MinAlpha) yield break;
                var imageColor = color;
                imageColor.a += flashSettings.Speed * Time.deltaTime * (show ? 1 : -1);
                image.color = imageColor;
                yield return null;
            }
        }
    }
}
