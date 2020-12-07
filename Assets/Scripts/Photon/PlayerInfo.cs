using System;
using UnityEngine;

namespace Photon
{
    public class PlayerInfo : MonoBehaviour
    {
        [SerializeField] private GameObject[] networkPlayerCharacters;
        [SerializeField] private GameObject[] localPlayerCharacters;

        public static PlayerInfo Instance;

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

            DontDestroyOnLoad(this);
        }

        public GameObject GetNetworkCharacter(int playerNumber) =>
            networkPlayerCharacters[playerNumber % networkPlayerCharacters.Length];

        public GameObject GetLocalCharacter(int playerNumber) =>
            localPlayerCharacters[playerNumber % localPlayerCharacters.Length];
    }
}
