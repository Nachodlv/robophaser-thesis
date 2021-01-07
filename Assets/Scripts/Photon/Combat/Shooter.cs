using System;
using System.Collections;
using System.IO;
using Photon.CustomPunPool;
using Photon.GameControllers;
using Photon.Pun;
using UnityEngine;

namespace Photon.Combat
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

        private int _currentShootingPoint;
        private ShootingPoint[] _shootingPoints;
        private string _bulletId = Path.Combine("PhotonPrefabs", "Blue Bullet");

        private ShootingPoint[] ShootingPoints =>
            _shootingPoints ?? (_shootingPoints = transform.parent.GetComponentsInChildren<ShootingPoint>());

        public float MaxForce => maxForce;
        public float MinForce => minForce;
        public float AddForceVelocity => addForceVelocity;
        public int MaxClipAmmo => maxClipAmmo;

        private void Awake()
        {
            _currentClipAmmo = maxClipAmmo;
            _reloadingWaitTime = new WaitForSeconds(reloadingTime);
            _startReloadingCoroutine = StartReloadingCoroutine;

            // Initialize the first bullets
            if (PhotonNetwork.IsMasterClient)
                PunPool.Instance.CreateInstances(_bulletId,
                    (int) (maxClipAmmo * PhotonRoom.Instance.Settings.maxPlayers * 1.5));
        }

        public void Shoot(float force)
        {
            var now = Time.time;
            if (!CanShoot(now)) return;
            _lastShoot = now;
            _currentClipAmmo -= 1;
            OnAmmoChange?.Invoke(_currentClipAmmo);
            photonView.RPC(nameof(RPC_SpawnBullet), RpcTarget.MasterClient,
                PhotonNetwork.LocalPlayer.UserId,
                force,
                ShootingPoints[_currentShootingPoint].Transform.position,
                GetShootingDirection()
            );
            _currentShootingPoint = (_currentShootingPoint + 1) % ShootingPoints.Length;
        }

        public void Reload()
        {
            if (CanReload()) StartCoroutine(_startReloadingCoroutine());
        }

        public bool CanReload()
        {
            return _currentClipAmmo < maxClipAmmo && !_reloading;
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

        [PunRPC]
        private void RPC_SpawnBullet(string shooterId, float force, Vector3 position, Vector3 direction)
        {
            var bullet = PunPool.Instance.Instantiate(
                _bulletId,
                position,
                Quaternion.LookRotation(direction)).GetComponent<Bullet>();
            Debug.Log($"##### Bullet: {bullet.name}");
            bullet.Shoot(shooterId, direction * force);
        }

        private Vector3 GetShootingDirection()
        {
            var shootingPoint = ShootingPoints[_currentShootingPoint].Transform;
            var shootingPointPosition = shootingPoint.position;
            var ray = new Ray(shootingPointPosition, shootingPoint.forward);
            var target = Physics.Raycast(ray, out var hit) ? hit.point : ray.GetPoint(10);
            var direction = (target - shootingPointPosition).normalized;
            return direction;
        }
    }
}
