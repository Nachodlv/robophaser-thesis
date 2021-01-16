using System;
using System.Collections;
using Photon;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Combat
{
    public class ShootButton : MonoBehaviour
    {
        [SerializeField] private Button button;

        private void Awake()
        {
            button.onClick.AddListener(Shoot);
        }

        public void Show()
        {
            var shooter = PhotonRoom.Instance.LocalPlayer.Shooter;
            shooter.OnAmmoChange += AmmoChange;
            shooter.OnStartReloading += StartReloading;
            shooter.OnStopReloading += StopReloading;
        }

        private void Shoot()
        {
            PhotonRoom.Instance.LocalPlayer.Shooter.Shoot();
        }

        private void AmmoChange(int newAmmo)
        {
            if (newAmmo == 0)
            {
                button.interactable = false;
            } else if (!button.interactable)
            {
                button.interactable = true;
            }
        }

        private void StartReloading()
        {
            button.interactable = false;
        }

        private void StopReloading()
        {
            button.interactable = true;
        }
    }
}
