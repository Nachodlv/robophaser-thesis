using System;
using System.IO;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SpatialTracking;
using Random = UnityEngine.Random;

namespace Photon.GameControllers
{
    public class PhotonPlayer : MonoBehaviourPun
    {
        [SerializeField] private int maxHealth;

        private TrackedPoseDriver _camera;
        private Transform _playerAvatar;
        private Shooter _shooter;
        private int _currentHealth;

        public Shooter Shooter
        {
            get
            {
                if(_shooter == null) _shooter = _playerAvatar.GetComponentInChildren<Shooter>();
                return _shooter;
            }
        }

        public int MaxHealth => maxHealth;

        public Transform CameraTransform => _camera.transform;

        public delegate void HealthUpdateCallback(int currentHealth);
        public event HealthUpdateCallback OnHealthUpdate;

        private void Awake()
        {
            if (!photonView.IsMine) return;

            _currentHealth = maxHealth;
            _playerAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerAvatar"),
                Vector3.zero, Quaternion.identity).transform;

            _camera = FindObjectOfType<TrackedPoseDriver>();
            if (_camera == null) Debug.LogError("First person camera not found!");

            PhotonRoom.Instance.AddPhotonPlayer(this);
        }

        private void Update()
        {
            // TODO remove theses methods when player movement is tested
            // if (!photonView.IsMine) return;
            // var cameraTransform = _camera.transform;
            // SetUpRotation(cameraTransform);
            // SetUpPosition(cameraTransform);
        }

        public void ReceiveDamage(int damage)
        {
            _currentHealth -= damage;
            OnHealthUpdate?.Invoke(_currentHealth);
        }

        private void SetUpRotation(Transform cameraTransform)
        {
            _playerAvatar.rotation = cameraTransform.rotation;
        }

        private void SetUpPosition(Transform cameraTransform)
        {
            _playerAvatar.position = cameraTransform.position;
        }
    }
}
