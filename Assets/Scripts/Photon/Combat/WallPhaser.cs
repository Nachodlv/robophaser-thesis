using System;
using System.Collections.Generic;
using UnityEngine;

namespace Photon.Combat
{
    public class WallPhaser : MonoBehaviour
    {
        [SerializeField] private float maxEnergy;
        [SerializeField] private float energyRegen;
        [SerializeField] private float energyDecrease;
        [SerializeField] private StatusEffects statusEffect;
        [SerializeField] private int damageWhenNoEnergy;
        [SerializeField] private LayerMask obstacleLayer;

        private List<Collider> _colliders;
        private bool _applyingDamage;
        private float _currentEnergy;
        private float _statusEffectId;

        public delegate void EnergyUpdateCallback(float energy);
        public event EnergyUpdateCallback OnEnergyUpdate;

        public float MaxEnergy => maxEnergy;

        private void Awake()
        {
            _currentEnergy = maxEnergy;
            _colliders = new List<Collider>(3);
        }

        private void Update()
        {
            if (_colliders.Count > 0)
            {
                if (_applyingDamage) return;

                if(_currentEnergy <= 0)
                {
                    _statusEffectId = statusEffect.AddStatusEffect(damageWhenNoEnergy);
                    _applyingDamage = true;
                }
                else
                {
                    _currentEnergy -= energyDecrease * Time.deltaTime;
                    OnEnergyUpdate?.Invoke(_currentEnergy);
                }

                return;
            }

            if (_currentEnergy >= maxEnergy) return;
            _currentEnergy += energyRegen * Time.deltaTime;
            _currentEnergy = Math.Min(_currentEnergy, maxEnergy);
            OnEnergyUpdate?.Invoke(_currentEnergy);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (1 << other.gameObject.layer == obstacleLayer)
            {
                _colliders.Add(other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (1 << other.gameObject.layer != obstacleLayer || !_colliders.Contains(other)) return;
            _colliders.Remove(other);
            if (_colliders.Count != 0 || !_applyingDamage) return;
            statusEffect.RemoveStatusEffect(_statusEffectId);
            _applyingDamage = false;
        }
    }
}
