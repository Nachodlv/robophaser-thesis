using Photon;
using Photon.GameControllers;
using UnityEngine;
using Utils;

namespace UI.Combat
{
    public class OpponentHealthDisplayer : MonoBehaviour
    {
        [SerializeField] private HealthDisplayer healthDisplayer;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float timeShowingHealth = 2f;

        private Transform _localPlayer;
        private WaitSeconds _waitForHiding;

        private bool _showing;

        private void Awake()
        {
            ChangeVisibility(false);
            _waitForHiding = new WaitSeconds(this, () => ChangeVisibility(false), timeShowingHealth);
        }

        private void Start()
        {
            var photonPlayer = GetComponentInParent<AvatarSetup>().PhotonPlayer;
            healthDisplayer.DisplayHealth(photonPlayer);
            photonPlayer.OnHealthUpdate += ShowHealth;
            _localPlayer = PhotonRoom.Instance.LocalPlayer.CameraTransform;
        }

        private void Update()
        {
            if(_showing) transform.LookAt(_localPlayer);
        }

        private void ShowHealth(int newHealth)
        {
            ChangeVisibility(true);
            _waitForHiding.Stop();
            _waitForHiding.Wait();
        }

        private void ChangeVisibility(bool show)
        {
            if (_showing && show) return;
            canvasGroup.interactable = show;
            canvasGroup.blocksRaycasts = show;
            canvasGroup.alpha = show ? 1 : 0;
            _showing = show;

        }
    }
}
