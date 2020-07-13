using System.IO;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Photon
{
    [RequireComponent(typeof(PhotonView))]
    public class PhotonRoom : MonoBehaviourPunCallbacks, IInRoomCallbacks
    {
        [SerializeField, Tooltip("Scene index that will be loaded when the game starts")]
        private int multiplayerScene;

        public static PhotonRoom Room;
        
        private PhotonView _photonView;
        private int _currentScene;

        private void Awake()
        {
            if (Room == null)
            {
                Room = this;
            }
            else if (Room != this)
            {
                Destroy(Room.gameObject);
                Room = this;
            }

            DontDestroyOnLoad(gameObject);
            _photonView = GetComponent<PhotonView>();
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
            StartGame();
        }

        private void OnSceneFinishLoading(Scene scene, LoadSceneMode mode)
        {
            _currentScene = scene.buildIndex;
            if (_currentScene == multiplayerScene)
            {
                CreatePlayer();
            }
        }

        private void CreatePlayer()
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonPlayer"), transform.position,
                Quaternion.identity);
        }

        private void StartGame()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            PhotonNetwork.LoadLevel(multiplayerScene);
        }
    }
}