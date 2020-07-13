using System;
using UnityEngine;

namespace Photon.GameControllers
{
    public class GameSetup : MonoBehaviour
    {
        [SerializeField] private Transform[] spawnPoints;
        
        public static GameSetup Instance;

        public Transform[] SpawnPoints => spawnPoints;

        private void OnEnable()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
    }
}