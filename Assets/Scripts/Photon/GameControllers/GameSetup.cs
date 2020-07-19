using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Photon.GameControllers
{
    public class GameSetup : MonoBehaviour
    {
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private MultiplayerSettings settings;

        public static GameSetup Instance;

        public Transform[] SpawnPoints => spawnPoints;

        private void OnEnable()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void DisconnectPlayer()
        {
            StartCoroutine(DisconnectAndLoad());
        }

        private IEnumerator DisconnectAndLoad()
        {
            PhotonNetwork.Disconnect();
            while (PhotonNetwork.IsConnected)
            {
                yield return null;
            }

            SceneManager.LoadScene(settings.mainMenuScene);
        }
    }
}
