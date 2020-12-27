using System.Threading.Tasks;
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

        [Header("UI - Remote player")]
        [SerializeField] private HealthDisplayer remotePlayerHealth;

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
            if(TryGetRemotePlayer(out var remotePlayer)) remotePlayerHealth.DisplayHealth(remotePlayer);
            shootButton.Show();
            reloadButton.Show();
            bulletDisplayer.Show();
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

        private static bool TryGetRemotePlayer(out PhotonPlayer remotePlayer)
        {
            var localPlayer = PhotonRoom.Instance.LocalPlayer;
            remotePlayer = default;
            foreach (var photonPlayer in FindObjectsOfType<PhotonPlayer>())
            {
                if (photonPlayer == localPlayer) continue;
                remotePlayer = photonPlayer;
                return true;
            }
            return false;
        }
    }
}
