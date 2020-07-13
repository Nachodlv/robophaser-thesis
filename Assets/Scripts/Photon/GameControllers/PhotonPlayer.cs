using System;
using System.IO;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Photon.GameControllers
{
    [RequireComponent(typeof(PhotonView))]
    public class PhotonPlayer : MonoBehaviourPun
    {
        private GameObject _playerAvatar;

        private void Awake()
        {
            if (!photonView.IsMine) return;
            var spawnPoint = GetRandomSpawnPoint();
            _playerAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerAvatar"),
                spawnPoint.position, spawnPoint.rotation);
        }

        private Transform GetRandomSpawnPoint()
        {
            var spawnPicked = Random.Range(0, GameSetup.Instance.SpawnPoints.Length);
            return GameSetup.Instance.SpawnPoints[spawnPicked].transform;
        }
    }
}