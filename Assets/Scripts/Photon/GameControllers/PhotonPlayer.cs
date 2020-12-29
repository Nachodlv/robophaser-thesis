using System;
using System.Collections;
using System.IO;
using Cues.Animations;
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
        private RobotOrbAnimator _animator;

        public Shooter Shooter => _shooter != null
            ? _shooter
            : _shooter = _playerAvatar.GetComponentInChildren<Shooter>();

        public RobotOrbAnimator Animator => _animator != null
            ? _animator
            : _animator = _playerAvatar.GetComponentInChildren<RobotOrbAnimator>();

        public int MaxHealth => maxHealth;
        public Transform CameraTransform => _camera.transform;

        public delegate void HealthUpdateCallback(int currentHealth);

        public event HealthUpdateCallback OnHealthUpdate;

        private void Awake()
        {
            _currentHealth = maxHealth;

            if (!photonView.IsMine) return;

            _playerAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerAvatar"),
                Vector3.zero, Quaternion.identity).transform;

            _camera = FindObjectOfType<TrackedPoseDriver>();
            if (_camera == null) Debug.LogError("First person camera not found!");

            PhotonRoom.Instance.AddPhotonPlayer(this);
        }

        public void ReceiveDamage(int damage)
        {
            photonView.RPC(nameof(RPC_ReceiveDamage), RpcTarget.All, damage);
        }

        [PunRPC]
        private void RPC_ReceiveDamage(int damage)
        {
            _currentHealth -= damage;
            Animator.TakeDamage();
            OnHealthUpdate?.Invoke(_currentHealth);
            Debug.Log($"##### New {(photonView.IsMine ? "Own" : "Enemy")} Health: {_currentHealth}");
        }
    }
}
