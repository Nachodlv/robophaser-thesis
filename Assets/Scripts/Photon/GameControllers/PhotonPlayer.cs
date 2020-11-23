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
        private TrackedPoseDriver _camera;
        private Transform _playerAvatar;
        private Shooter _shooter;

        public Shooter Shooter
        {
            get
            {
                if(_shooter == null) _shooter = _playerAvatar.GetComponentInChildren<Shooter>();
                return _shooter;
            }
        }

        public Transform CameraTransform => _camera.transform;

        private void Awake()
        {
            if (!photonView.IsMine) return;

            _playerAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerAvatar"),
                Vector3.zero, Quaternion.identity).transform;

            _camera = FindObjectOfType<TrackedPoseDriver>();
            if (_camera == null) Debug.LogError("First person camera not found!");

            PhotonRoom.Instance.PhotonPlayers.Add(this);
        }

        private void Update()
        {
            if (!photonView.IsMine) return;
            var cameraTransform = _camera.transform;
            SetUpRotation(cameraTransform);
            SetUpPosition(cameraTransform);
        }

        private void SetUpRotation(Transform cameraTransform)
        {
            var rotation = _playerAvatar.rotation;
            _playerAvatar.rotation = new Quaternion(rotation.x, cameraTransform.rotation.y, rotation.z, rotation.w);
        }

        private void SetUpPosition(Transform cameraTransform)
        {
            var cameraPosition = cameraTransform.position;
            var position = _playerAvatar.position;
            _playerAvatar.position = new Vector3(cameraPosition.x, position.y, cameraPosition.z);
        }
    }
}
