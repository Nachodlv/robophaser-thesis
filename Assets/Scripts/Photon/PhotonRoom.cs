using System;
using System.IO;
using Photon.Pun;
using Photon.Realtime;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Photon
{
    [RequireComponent(typeof(PhotonView))]
    public class PhotonRoom : MonoBehaviourPunCallbacks
    {
        [SerializeField] private MultiplayerSettings settings;
        [SerializeField] private float waitTimeWhenFull;

        public event Action OnPlayerLeft;

        public static PhotonRoom Instance;

        private int _currentScene;
        private bool _isGameLoaded;
        private Player[] _photonPlayers;
        private int _playersInGame;

        private bool _readyToStart;
        private float _timeToStart;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(Instance.gameObject);
                Instance = this;
            }

            DontDestroyOnLoad(gameObject);

            RestartTimer();
        }

        private void Update()
        {
            if (_isGameLoaded || !_readyToStart) return;

            _timeToStart -= Time.deltaTime;
            Debug.Log($"Time to start the game: {_timeToStart}");
            if (_timeToStart <= 0) StartGame();

        }

        public override void OnEnable()
        {
            base.OnEnable();
            PhotonNetwork.AddCallbackTarget(this);
            SceneManager.sceneLoaded += OnSceneFinishLoading;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            PhotonNetwork.RemoveCallbackTarget(this);
            SceneManager.sceneLoaded -= OnSceneFinishLoading;
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            _photonPlayers = PhotonNetwork.PlayerList;
            Loader.Instance.StopLoading();
            Debug.Log($"Player joined a room! Id: {PhotonNetwork.CurrentRoom.Name}");
            PlayerJoinedRoom();
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
            PlayerJoinedRoom();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            Debug.Log($"{otherPlayer.NickName} has left the game");
            SceneLoader.Instance.LoadSceneAsync(settings.mainMenuScene);
            PhotonNetwork.LeaveRoom();
            Destroy(gameObject);
        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            OnPlayerLeft?.Invoke();
            RestartTimer();
        }

        public void InstancePlayer()
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonPlayer"), Vector3.zero, Quaternion.identity);
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        private void OnSceneFinishLoading(Scene scene, LoadSceneMode mode)
        {
            _currentScene = scene.buildIndex;
            if (_currentScene != settings.multiplayerScene) return;
            _isGameLoaded = true;
        }

        private void PlayerJoinedRoom()
        {
            _photonPlayers = PhotonNetwork.PlayerList;
            Debug.Log($"Waiting for players to join ({_photonPlayers.Length} : {settings.maxPlayers})");
            if (_photonPlayers.Length < settings.maxPlayers) return;
            _readyToStart = true;
            if (!PhotonNetwork.IsMasterClient) return;
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }

        private void StartGame()
        {
            _isGameLoaded = true;
            _timeToStart = waitTimeWhenFull;
            if (!PhotonNetwork.IsMasterClient)
                return;
            PhotonNetwork.CurrentRoom.IsOpen = false;
            SceneLoader.Instance.LoadSceneAsync(settings.multiplayerScene);
        }

        private void RestartTimer()
        {
            _readyToStart = false;
            _isGameLoaded = false;
            _timeToStart = waitTimeWhenFull;
        }

    }
}
