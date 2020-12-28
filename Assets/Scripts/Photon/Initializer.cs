using System;
using System.IO;
using Photon.Pun;
using UnityEngine;

namespace Photon
{
    public class Initializer : MonoBehaviour
    {
        [SerializeField] private bool initializeRematchManager = true;
        [SerializeField] private bool initializeParticleEffectPooler = true;

        private void Awake()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            if (initializeRematchManager)
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "RematchManager"), Vector3.zero,
                    Quaternion.identity);
            if (initializeParticleEffectPooler)
                PhotonNetwork.Instantiate(Path.Combine("FX", "Particle Effect Pooler"), Vector3.zero,
                    Quaternion.identity);
        }
    }
}
