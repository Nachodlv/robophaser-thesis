using System;
using System.Collections.Generic;
using Cues;
using Photon.Pun;
using UnityEngine;

namespace Photon.Combat
{
    public class WallPhaser : MonoBehaviourPun
    {
        [SerializeField] private float maxEnergy = 100f;
        [SerializeField] private float energyRegen = 20f;
        [SerializeField] private float energyDecrease = 30f;
        [SerializeField] private StatusEffects statusEffect;
        [SerializeField] private int damageWhenNoEnergy = 1;
        [SerializeField] private LayerMask obstacleLayer;
        [SerializeField] private Cue phasingCue;

        private List<Collider> _colliders;
        private bool _applyingDamage;
        private float _currentEnergy;
        private float _statusEffectId;

        public delegate void EnergyUpdateCallback(float energy);
        public event EnergyUpdateCallback OnEnergyUpdate;

        public float MaxEnergy => maxEnergy;

        private void Awake()
        {
            if(!photonView.IsMine) Destroy(this);
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
                if (_colliders.Count == 0)
                {
                    phasingCue.Execute(transform.position, Quaternion.identity);
                }
                _colliders.Add(other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (1 << other.gameObject.layer != obstacleLayer || !_colliders.Contains(other)) return;
            _colliders.Remove(other);
            if (_colliders.Count > 0) return;
            phasingCue.StopExecution();
            if (!_applyingDamage) return;
            statusEffect.RemoveStatusEffect(_statusEffectId);
            _applyingDamage = false;
        }

    }
}
