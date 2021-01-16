#define SHOW_GIZMOS

using System;
using System.Collections;
using System.IO;
using Cues;
using Photon.CustomPunPool;
using Photon.GameControllers;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;

namespace Photon.Combat
{
    public class Shooter : MonoBehaviourPun
    {
        [SerializeField] private float addForceVelocity;
        [SerializeField] private float timeBetweenShoots = 0.5f;
        [SerializeField] private float reloadingTime = 2f;
        [SerializeField] private int maxClipAmmo = 3;
        [SerializeField] private LayerMask targetLayer;
        [SerializeField] private Cue shootCue;
        [SerializeField] private Cue reloadCue;

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
        private RaycastHit[] _raycastHits = new RaycastHit[10];
        private Camera _camera;

        private int _currentShootingPoint;
        private ShootingPoint[] _shootingPoints;
        private string _bulletId = Path.Combine("PhotonPrefabs", "Blue Bullet");

        private ShootingPoint[] ShootingPoints =>
            _shootingPoints != null && _shootingPoints.Length > 0
                ? _shootingPoints
                : _shootingPoints = transform.parent.GetComponentsInChildren<ShootingPoint>();

        public float AddForceVelocity => addForceVelocity;
        public int MaxClipAmmo => maxClipAmmo;

        private void Awake()
        {
            _currentClipAmmo = maxClipAmmo;
            _reloadingWaitTime = new WaitForSeconds(reloadingTime);
            _startReloadingCoroutine = StartReloadingCoroutine;
            _camera = Camera.main;
            // Initialize the first bullets
            if (PhotonNetwork.IsMasterClient)
                PunPool.Instance.CreateInstances(_bulletId,
                    (int) (maxClipAmmo * PhotonRoom.Instance.Settings.maxPlayers * 1.5));
        }

        public void Shoot()
        {
            var now = Time.time;
            if (!CanShoot(now)) return;
            _lastShoot = now;
            _currentClipAmmo -= 1;
            OnAmmoChange?.Invoke(_currentClipAmmo);
            var shootingPosition = ShootingPoints[_currentShootingPoint].Transform.position;
            photonView.RPC(nameof(RPC_SpawnBullet), RpcTarget.MasterClient,
                PhotonNetwork.LocalPlayer.UserId,
                addForceVelocity,
                PhotonNetwork.LocalPlayer.GetPlayerNumber(),
                shootingPosition,
                GetShootingDirection()
            );
            shootCue.Execute(shootingPosition, Quaternion.identity);
            _currentShootingPoint = (_currentShootingPoint + 1) % ShootingPoints.Length;
        }

        public void Reload()
        {
            if (!CanReload()) return;
            StartCoroutine(_startReloadingCoroutine());
            reloadCue.Execute(transform.position, Quaternion.identity);
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
        private void RPC_SpawnBullet(string shooterId, float force, int playerNumber, Vector3 position,
            Vector3 direction)
        {
            var bullet = PunPool.Instance.Instantiate(
                _bulletId,
                position,
                Quaternion.LookRotation(direction)).GetComponent<Bullet>();
            Debug.Log($"##### Bullet: {bullet.name}");
            bullet.Shoot(shooterId, playerNumber, direction * force);
        }

        private Vector3 GetShootingDirection()
        {
            var shootingPoint = ShootingPoints[_currentShootingPoint].Transform;
            var shootingPointPosition = shootingPoint.position;
            var cameraTransform = _camera.transform;
            var ray = new Ray(cameraTransform.position, cameraTransform.forward);
            var size = Physics.RaycastNonAlloc(ray, _raycastHits, 100, targetLayer);
            var target = Vector3.zero;
            var setTarget = false;
            for (var i = 0; i < size; i++)
            {
                if (_raycastHits[i].transform != transform.parent)
                {
                    target = _raycastHits[i].point;
                    setTarget = true;
                    break;
                }
            }
            if (!setTarget) target = ray.GetPoint(10);
            var direction = (target - shootingPointPosition).normalized;

#if SHOW_GIZMOS
            _gizmosShootingTarget = target;
            _showGizmos = true;
#endif
            return direction;
        }

#if SHOW_GIZMOS
        private Vector3 _gizmosShootingTarget;
        private bool _showGizmos;

        private void OnDrawGizmos()
        {
            if (!_showGizmos) return;

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_gizmosShootingTarget, 0.05f);
        }
#endif
    }
}
