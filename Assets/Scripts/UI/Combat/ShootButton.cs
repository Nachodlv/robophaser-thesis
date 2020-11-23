using System;
using System.Collections;
using Photon;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Combat
{
    public class ShootButton : MonoBehaviour
    {
        [SerializeField] private ButtonWithEvents button;
        [SerializeField] private StatBar forceDisplay;

        private float _currentForce;
        private Func<IEnumerator> _increaseForce;
        private Coroutine _increasingForce;

        private void Awake()
        {
            button.PointerDown += StartApplyingForce;
            button.PointerUp += Shoot;
            _increaseForce = ChangeForce;
        }

        public void Show()
        {
            var shooter = PhotonRoom.Instance.LocalPlayer.Shooter;
            shooter.OnAmmoChange += AmmoChange;
            shooter.OnStartReloading += StartReloading;
            shooter.OnStopReloading += StopReloading;
        }

        private void StartApplyingForce()
        {
            var shooter = PhotonRoom.Instance.LocalPlayer.Shooter;
            _currentForce = shooter.MinForce;
            forceDisplay.MaxValue = shooter.MaxForce - shooter.MinForce;
            _increasingForce = StartCoroutine(_increaseForce());
        }

        private void Shoot()
        {
            var shooter = PhotonRoom.Instance.LocalPlayer.Shooter;
            shooter.Shoot(_currentForce);
            if(_increasingForce != null) StopCoroutine(_increasingForce);
            _currentForce = shooter.MinForce;
            forceDisplay.CurrentValue = 0;
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


        private IEnumerator ChangeForce()
        {
            var shooter = PhotonRoom.Instance.LocalPlayer.Shooter;
            var addForceVelocity = shooter.AddForceVelocity;
            while (_currentForce <= shooter.MaxForce)
            {
                _currentForce += addForceVelocity * Time.deltaTime;
                forceDisplay.CurrentValue = _currentForce - shooter.MinForce;
                yield return null;
            }

        }
    }
}
