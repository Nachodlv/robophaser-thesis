using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Utils
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Fader : MonoBehaviour
    {
        [SerializeField] private float fadeSpeed = 1;
        [SerializeField] private bool showOnAwake = true;

        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            ChangeVisibility(showOnAwake);
        }

        public async Task FadeIn()
        {
            while (_canvasGroup.alpha > 0)
            {
                _canvasGroup.alpha += Time.deltaTime * fadeSpeed;
                await Task.Yield();
            }
            ChangeVisibility(true);
        }

        public async Task FadeOut()
        {
            while (_canvasGroup.alpha < 1)
            {
                _canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
                await Task.Yield();
            }
            ChangeVisibility(false);
        }

        private void ChangeVisibility(bool show)
        {
            _canvasGroup.alpha = show ? 1 : 0;
            _canvasGroup.interactable = show;
            _canvasGroup.blocksRaycasts = show;
        }
    }
}
