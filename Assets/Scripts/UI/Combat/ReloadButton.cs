using System;
using Photon;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Combat
{
    public class ReloadButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Animator animator;
        private static readonly int Reloading = Animator.StringToHash("reloading");

        private void Awake()
        {
            button.onClick.AddListener(Reload);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(Reload);
        }

        public void Show()
        {
            var shooter = PhotonRoom.Instance.LocalPlayer.Shooter;
            shooter.OnAmmoChange += AmmoChange;
            shooter.OnStartReloading += StartReloading;
            shooter.OnStopReloading += StopReloading;
        }

        private void Reload()
        {
            PhotonRoom.Instance.LocalPlayer.Shooter.Reload();
        }

        private void AmmoChange(int newAmmo)
        {
            if (!PhotonRoom.Instance.LocalPlayer.Shooter.CanReload())
            {
                button.interactable = false;
            } else if (!button.interactable)
            {
                button.interactable = true;
            }
        }

        private void StartReloading()
        {
            animator.SetBool(Reloading, true);
        }

        private void StopReloading()
        {
            animator.SetBool(Reloading, false);
        }
    }
}
