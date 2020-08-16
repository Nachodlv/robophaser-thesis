using System;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Loader : MonoBehaviour
    {
        public static Loader Instance;

        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            Instance = this;
            _canvasGroup = GetComponent<CanvasGroup>();
            ChangeVisibility(false);
        }

        public void StartLoading()
        {
            ChangeVisibility(true);
        }

        public void StopLoading()
        {
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
