using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Photon.Combat
{
    struct StatusEffect
    {
        public int DamagePerSecond { get; }
        private float Id { get; }

        public StatusEffect(int damagePerSecond, float id)
        {
            DamagePerSecond = damagePerSecond;
            Id = id;
        }
    }

    [RequireComponent(typeof(IDamageTaker))]
    public class StatusEffects : MonoBehaviour
    {
        private IDamageTaker _damageTaker;
        private readonly Dictionary<float, StatusEffect> _statusEffects = new Dictionary<float, StatusEffect>();
        private Func<IEnumerator> _statusEffectsUpdate;
        private WaitForSeconds _oneSecond;
        private bool _coroutineRunning;

        private void Awake()
        {
            _damageTaker = GetComponent<IDamageTaker>();
            _statusEffectsUpdate = StatusEffectsUpdate;
            _oneSecond = new WaitForSeconds(1);
        }

        public float AddStatusEffect(int damagePerSecond)
        {
            var id = Time.time;
            _statusEffects.Add(id, new StatusEffect(damagePerSecond, id));
            if (!_coroutineRunning) StartCoroutine(_statusEffectsUpdate());
            return id;
        }

        public void RemoveStatusEffect(float id)
        {
            if (!_statusEffects.ContainsKey(id)) return;
            _statusEffects.Remove(id);
        }

        private IEnumerator StatusEffectsUpdate()
        {
            yield return _oneSecond;

            if (_statusEffects.Count == 0)
            {
                _coroutineRunning = false;
                yield break;
            }

            _coroutineRunning = true;
            foreach (var statusEffect in _statusEffects.Values)
            {
                _damageTaker.TakeDamage(statusEffect.DamagePerSecond);
            }

            StartCoroutine(_statusEffectsUpdate());
        }
    }
}
