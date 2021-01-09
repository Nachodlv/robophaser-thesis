using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

namespace Utils.Pools
{
    public enum ParticleType
    {
        LaserImpactBlue
    }

    [Serializable]
    public class ParticleReferences
    {
        public ParticleType type;
        public ParticleEffectPooleable particleEffect;
        public int quantity = 10;
    }
    public class ParticleEffectPooler: PunSingleton<ParticleEffectPooler>
    {
        [SerializeField] private ParticleReferences[] particles;

        private Dictionary<ParticleType, ObjectPooler<ParticleEffectPooleable>> _pools;

        protected override void Awake()
        {
            base.Awake();
            _pools = new Dictionary<ParticleType, ObjectPooler<ParticleEffectPooleable>>();
            foreach (var particle in particles)
            {
                var pooler = new ObjectPooler<ParticleEffectPooleable>();
                pooler.InstantiateObjects(particle.quantity, particle.particleEffect,
                    $"Pool of {particle.particleEffect.name}");
                _pools.Add(particle.type, pooler);
            }
        }

        public void PlayParticleEffect(ParticleType type, Vector3 position, Quaternion rotation)
        {
            photonView.RPC(nameof(RPC_PlayParticleEffect), RpcTarget.All, type, position, rotation);
        }

        [PunRPC]
        private void RPC_PlayParticleEffect(ParticleType type, Vector3 position, Quaternion rotation)
        {
            var particle = _pools[type]?.GetNextObject();
            if (particle != null) particle.transform.SetPositionAndRotation(position, rotation);
        }

    }
}
