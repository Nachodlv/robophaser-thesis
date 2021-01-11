using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ImageFlash : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private float speed = 0.4f;
        [SerializeField] private float maxAlpha = 0.15f;
        [SerializeField] private float minAlpha = 0;
        [SerializeField] private int quantityOfFlashes = 1;

        private Func<IEnumerator> _startFlashingCached;
        private Func<bool, IEnumerator> _changeFlashVisibilityCached;
        private bool _flashing;
        private Coroutine _startFlashingCoroutine;

        private void Awake()
        {
            _startFlashingCached = StartFlashing;
            _changeFlashVisibilityCached = ChangeFlashVisibility;
        }

        public void Flash()
        {
            if(_flashing && _startFlashingCoroutine != null) StopCoroutine(_startFlashingCoroutine);
            _startFlashingCoroutine = StartCoroutine(_startFlashingCached());
        }

        private IEnumerator StartFlashing()
        {
            _flashing = true;
            var showing = true;
            for (var i = 0; i < quantityOfFlashes * 2; i++)
            {
                yield return _changeFlashVisibilityCached(showing);
                showing = !showing;
            }
            _flashing = false;
        }

        private IEnumerator ChangeFlashVisibility(bool show)
        {
            while (true)
            {
                var color = image.color;
                if (show && color.a >= maxAlpha) yield break;
                if (!show && color.a <= minAlpha) yield break;
                var imageColor = color;
                imageColor.a += speed * Time.deltaTime * (show ? 1 : -1);
                image.color = imageColor;
                yield return null;
            }
        }
    }
}
