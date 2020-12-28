using System;
using System.IO;
using Photon.Pun;
using UnityEngine;

namespace Photon
{
    public class Initializer : MonoBehaviour
    {
        [SerializeField] private bool initializeRematchManager = true;
        private void Awake()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            if(initializeRematchManager)
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "RematchManager"), Vector3.zero, Quaternion.identity);
        }
    }
}
