using System;
using System.Collections.Generic;
using System.Linq;
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
        public string particleEffect;
        public int quantity = 10;
    }
    public class ParticleEffectPooler: PunSingleton<ParticleEffectPooler>
    {
        [SerializeField] private ParticleReferences[] particles;

        public static ParticleEffectPooler Instance;

        private Dictionary<ParticleType, ObjectNetworkPooler<ParticleEffectPooleable>> _pools;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            _pools = new Dictionary<ParticleType, ObjectNetworkPooler<ParticleEffectPooleable>>();
            foreach (var particle in particles)
            {
                var pooler = new ObjectNetworkPooler<ParticleEffectPooleable>(particle.particleEffect);
                pooler.InstantiateObjects(particle.quantity, null,
                    $"Pool of {particle.particleEffect.Split('/').Last()}");
                _pools.Add(particle.type, pooler);
            }
        }

        public ParticleEffectPooleable GetParticleEffect(ParticleType type, Vector3 position, Quaternion rotation)
        {
            var particle = _pools[type]?.GetNextObject();
            if (particle == null) return null;
            particle.SetPosition(position, rotation);
            return particle;
        }

    }
}
