using System.Threading.Tasks;
using Photon;
using Photon.GameControllers;
using UnityEngine;
using Utils;

namespace UI.Combat
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] private RectTransform mainButtonSection;
        [SerializeField] private RectTransform secondaryButtonSection;
        [SerializeField] private ShootButton shootButton;
        [SerializeField] private ReloadButton reloadButton;
        [SerializeField] private Fader fader;

        private bool _reloadingButtonInMainSection;

        public void StartCombatPhase()
        {
            fader.FadeIn();
            PhotonRoom.Instance.LocalPlayer.Shooter.OnAmmoChange += AmmoChange;
            PhotonRoom.Instance.LocalPlayer.Shooter.OnStopReloading += StopReloading;
            shootButton.Show();
            reloadButton.Show();
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
