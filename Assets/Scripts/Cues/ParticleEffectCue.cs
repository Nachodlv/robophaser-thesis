using UnityEngine;
using Utils.Pools;

namespace Cues
{
    [CreateAssetMenu(fileName = "New Particle Effect Cue", menuName = "Cue/Particle Effect", order = 0)]
    public class ParticleEffectCue : Cue
    {
        [SerializeField] private ParticleType particleType;

        public override void Execute(Vector3 position, Quaternion rotation)
        {
            ParticleEffectPooler.Instance.PlayParticleEffect(particleType, position, rotation);
        }
    }
}
