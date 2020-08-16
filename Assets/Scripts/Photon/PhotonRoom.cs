using System;
using System.IO;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            Debug.Log("Player joined a room!");
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
        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            OnPlayerLeft?.Invoke();
            Destroy(gameObject);
        }

        private void OnSceneFinishLoading(Scene scene, LoadSceneMode mode)
        {
            _currentScene = scene.buildIndex;
            if (_currentScene != settings.multiplayerScene) return;
            _isGameLoaded = true;
            photonView.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient);
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
            OnGameStart?.Invoke();
            if (!PhotonNetwork.IsMasterClient)
                return;
            PhotonNetwork.CurrentRoom.IsOpen = false;

            // PhotonNetwork.LoadLevel(settings.multiplayerScene); TODO restore
            photonView.RPC("RPC_CreatePlayer", RpcTarget.All);
        }

        private void RestartTimer()
        {
            _readyToStart = false;
        }

        [PunRPC]
        private void RPC_LoadedGameScene()
        {
            _playersInGame++;
            if (_playersInGame == PhotonNetwork.PlayerList.Length)
            {
                photonView.RPC("RPC_CreatePlayer", RpcTarget.All);
            }
        }

        [PunRPC]
        private void RPC_CreatePlayer()
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonPlayer"), Vector3.zero, Quaternion.identity);
        }
    }
}
