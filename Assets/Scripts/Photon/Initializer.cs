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
        [SerializeField] private bool initializeCountdownManager = true;
        [SerializeField] private bool initializeAudioManager = true;

        private void Awake()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            if (initializeRematchManager)
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "RematchManager"), Vector3.zero,
                    Quaternion.identity);
            if (initializeParticleEffectPooler)
                PhotonNetwork.Instantiate(Path.Combine("FX", "Particle Effect Pooler"), Vector3.zero,
                    Quaternion.identity);
            if (initializeCountdownManager)
                PhotonNetwork.Instantiate(Path.Combine("Utils", "CountdownManager"), Vector3.zero, Quaternion.identity);
            if (initializeAudioManager)
                PhotonNetwork.Instantiate(Path.Combine("Utils", "AudioManager"), Vector3.zero, Quaternion.identity);
        }
    }
}
