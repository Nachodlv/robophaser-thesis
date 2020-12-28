using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace Utils.Pools
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleEffectPooleable: NetworkPooleable
    {
        private ParticleSystem _particleSystem;
        private Func<IEnumerator> _waitParticleToStop;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
            _waitParticleToStop = WaitParticleToStop;
        }

        public override void Activate()
        {
            base.Activate();
            _particleSystem.time = 0;
            _particleSystem.Play();
            StartCoroutine(_waitParticleToStop());
        }

        private IEnumerator WaitParticleToStop()
        {
            var main = _particleSystem.main;
            var clipLength = main.duration;
            clipLength += main.startLifetime.constantMax;
            var now = Time.time;
            while (Time.time - now < clipLength)
                yield return null;
            Deactivate();
        }

        public void SetPosition(Vector3 position, Quaternion rotation)
        {
            PhotonView.RPC(nameof(RPC_SetPosition), RpcTarget.All, position, rotation);
        }

        private void RPC_SetPosition(Vector3 position, Quaternion rotation)
        {
            var myTransform = transform;
            myTransform.position = position;
            myTransform.rotation = rotation;
        }
    }
}
