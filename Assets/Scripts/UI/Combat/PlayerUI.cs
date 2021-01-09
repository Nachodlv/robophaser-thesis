using Photon;
using Photon.GameControllers;
using UnityEngine;
using Utils;

namespace UI.Combat
{
    public class PlayerUI : MonoBehaviour
    {
        [Header("UI - Local player")]
        [SerializeField] private RectTransform mainButtonSection;
        [SerializeField] private RectTransform secondaryButtonSection;
        [SerializeField] private ShootButton shootButton;
        [SerializeField] private ReloadButton reloadButton;
        [SerializeField] private HealthDisplayer localPlayerHealth;
        [SerializeField] private BulletDisplayer bulletDisplayer;
        [SerializeField] private OutOfLimitsWarning outOfLimitsWarning;

        [Header("Components")]
        [SerializeField] private Fader fader;

        private bool _reloadingButtonInMainSection;

        public void StartCombatPhase()
        {
            fader.FadeIn();
            var localPlayer = PhotonRoom.Instance.LocalPlayer;
            localPlayer.Shooter.OnAmmoChange += AmmoChange;
            localPlayer.Shooter.OnStopReloading += StopReloading;
            localPlayerHealth.DisplayHealth(localPlayer);
            shootButton.Show();
            reloadButton.Show();
            bulletDisplayer.Show();
            outOfLimitsWarning.Show();
        }

        private void AmmoChange(int newAmmo)
        {
            if (newAmmo != 0) return;
            shootButton.gameObject.SetActive(false);
            reloadButton.transform.SetParent(mainButtonSection, false);
            _reloadingButtonInMainSection = true;
        }

        private void StopReloading()
        {
            if (!_reloadingButtonInMainSection) return;
            shootButton.gameObject.SetActive(true);
            reloadButton.transform.SetParent(secondaryButtonSection, false);
        }
    }
}
