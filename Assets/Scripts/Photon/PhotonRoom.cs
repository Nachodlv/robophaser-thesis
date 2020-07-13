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
        [SerializeField] private float startingTime;

        private static PhotonRoom _room;

        private int _currentScene;
        private bool _isGameLoaded;
        private Player[] _photonPlayers;
        private int _playersInRoom;
        private int _numberInRoom;
        private int _playerInGame;

        private bool _readyToCount;
        private bool _readyToStart;
        private float _lessThanMaxPlayers;
        private float _atMaxPlayers;
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
            if (settings.delayStart)
            {
                if (_playersInRoom == 1)
                {
                    RestartTimer();
                }

                if (!_isGameLoaded)
                {
                    if (_readyToStart)
                    {
                        _atMaxPlayers -= Time.deltaTime;
                        _lessThanMaxPlayers = _atMaxPlayers;
                        _timeToStart = _atMaxPlayers;
                    }
                    else if (_readyToCount)
                    {
                        _lessThanMaxPlayers -= Time.deltaTime;
                        _timeToStart = _lessThanMaxPlayers;
                    }

                    Debug.Log($"Time to start the game: {_timeToStart}");
                    if (_timeToStart <= 0) StartGame();
                }
            }
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
            Debug.Log("Player joined a room!");
            _photonPlayers = PhotonNetwork.PlayerList;
            _playersInRoom = _photonPlayers.Length;
            _numberInRoom = _playersInRoom;
            PhotonNetwork.NickName = _numberInRoom.ToString();
            if (settings.delayStart)
            {
                Debug.Log($"Waiting for players to join ({_playerInGame} : {settings.maxPlayers})");
                if (_playersInRoom > 1)
                {
                    _readyToCount = true;
                }

                if (_playersInRoom != settings.maxPlayers) return;
                _readyToStart = true;
                if (!PhotonNetwork.IsMasterClient) return;
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
            else
            {
                StartGame();
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
            _photonPlayers = PhotonNetwork.PlayerList;
            _playersInRoom++;
            if (settings.delayStart)
            {
                Debug.Log($"Waiting for players to join ({_playerInGame} : {settings.maxPlayers})");
                if (_playersInRoom > 1)
                {
                    _readyToCount = true;
                }

                if (_playersInRoom != settings.maxPlayers) return;
                _readyToStart = true;
                if (!PhotonNetwork.IsMasterClient) return;
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
            else
            {
                StartGame();
            }
        }

        private void OnSceneFinishLoading(Scene scene, LoadSceneMode mode)
        {
            _currentScene = scene.buildIndex;
            if (_currentScene == settings.multiplayerScene)
            {
                _isGameLoaded = true;
                if (settings.delayStart)
                {
                    photonView.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient);
                }
                else
                {
                    RPC_CreatePlayer();
                }
            }
        }

        private void StartGame()
        {
            _isGameLoaded = true;
            if (!PhotonNetwork.IsMasterClient) return;
            if (settings.delayStart)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }

            PhotonNetwork.LoadLevel(settings.multiplayerScene);
        }

        private void RestartTimer()
        {
            _lessThanMaxPlayers = startingTime;
            _timeToStart = startingTime;
            _atMaxPlayers = 6;
            _readyToCount = false;
            _readyToStart = false;
        }

        [PunRPC]
        private void RPC_LoadedGameScene()
        {
            _playerInGame++;
            if (_playerInGame == PhotonNetwork.PlayerList.Length)
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