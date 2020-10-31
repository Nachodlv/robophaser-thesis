using System;
using System.Collections;
using System.IO;
using JetBrains.Annotations;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Photon.GameControllers
{
    public class Shooter : MonoBehaviourPun
    {
        [SerializeField] private float maxForce;
        [SerializeField] private float minForce;
        [SerializeField] private float addForceVelocity;
        [SerializeField] private float timeBetweenShoots = 0.5f;
        [SerializeField] private float reloadingTime = 2f;
        [SerializeField] private int maxClipAmmo = 3;

        public delegate void AmmoChangeCallback(int currentClipAmmo);
        public event Action OnStartReloading;
        public event Action OnStopReloading;
        public event AmmoChangeCallback OnAmmoChange;

        private Action _opponentHit;
        private int _currentClipAmmo;
        private float _lastShoot;
        private float _currentForce;
        private bool _applyingForce;
        private bool _reloading;
        private WaitForSeconds _reloadingWaitTime;
        private Func<IEnumerator> _startReloadingCoroutine;

        public float MaxForce => maxForce;
        public float MinForce => minForce;

        public float AddForceVelocity => addForceVelocity;

        private void Awake()
        {
            _opponentHit = OpponentHit;
            _currentClipAmmo = maxClipAmmo;
            _reloadingWaitTime = new WaitForSeconds(reloadingTime);
            _startReloadingCoroutine = StartReloadingCoroutine;
        }

        public void Shoot(float force)
        {
            var now = Time.time;
            if (!CanShoot(now)) return;
            _lastShoot = now;
            _currentClipAmmo -= 1;
            OnAmmoChange?.Invoke(_currentClipAmmo);
            var cameraTransform = PhotonRoom.Instance.LocalPlayer.CameraTransform;
            var bullet = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Bullet"), cameraTransform.position,
                cameraTransform.rotation).GetComponent<Bullet>();
            bullet.Rigidbody.AddForce(cameraTransform.forward * force, ForceMode.Impulse);
            bullet.OnOpponentHit += _opponentHit;
        }

        public void Reload()
        {
            if(CanReload()) StartCoroutine(_startReloadingCoroutine());
        }

        public bool CanReload()
        {
            return _currentClipAmmo < maxClipAmmo && !_reloading;
        }

        private static void OpponentHit()
        {
            Debug.Log("Opponent Hit");
        }

        private bool CanShoot(float now)
        {
            return _currentClipAmmo > 0 && !_reloading && now - _lastShoot > timeBetweenShoots;
        }

        private IEnumerator StartReloadingCoroutine()
        {
            _reloading = true;
            OnStartReloading?.Invoke();
            yield return _reloadingWaitTime;
            _currentClipAmmo = maxClipAmmo;
            OnStopReloading?.Invoke();
            OnAmmoChange?.Invoke(_currentClipAmmo);
            _reloading = false;
        }
    }
}
