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

        public event Action OnPlayerJoined;
        public event Action OnPlayerLeft;
        public event Action OnGameStart;

        private static PhotonRoom _room;

        private int _currentScene;
        private bool _isGameLoaded;
        private Player[] _photonPlayers;
        private int _playersInGame;

        private bool _readyToStart;
        private float _timeToStart;

        private void Awake()
        {
            if (_room == null)
            {
                _room = this;
            }
            else if (_room != this)
            {
                Destroy(_room.gameObject);
                _room = this;
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
            OnPlayerJoined?.Invoke();
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

        private void OnSceneFinishLoading(Scene scene, LoadSceneMode mode)
        {
            _currentScene = scene.buildIndex;
            if (_currentScene != settings.multiplayerScene) return;
            _isGameLoaded = true;
            photonView.RPC(nameof(RPC_LoadedGameScene), RpcTarget.MasterClient);
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

        private async void StartGame()
        {
            _isGameLoaded = true;
            _timeToStart = waitTimeWhenFull;
            OnGameStart?.Invoke();
            if (!PhotonNetwork.IsMasterClient)
                return;
            PhotonNetwork.CurrentRoom.IsOpen = false;
            SceneLoader.Instance.LoadSceneAsync(settings.multiplayerScene);
            // photonView.RPC(nameof(RPC_CreatePlayer), RpcTarget.All);
        }

        private void RestartTimer()
        {
            _readyToStart = false;
            _isGameLoaded = false;
            _timeToStart = waitTimeWhenFull;
        }

        [PunRPC]
        private void RPC_LoadedGameScene()
        {
            _playersInGame++;
            if (_playersInGame == PhotonNetwork.PlayerList.Length)
            {
                photonView.RPC(nameof(RPC_CreatePlayer), RpcTarget.All);
            }
        }

        [PunRPC]
        private void RPC_CreatePlayer()
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonPlayer"), Vector3.zero, Quaternion.identity);
        }
    }
}
