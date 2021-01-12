using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Photon.Combat
{
    public class WallPhaser : MonoBehaviour
    {
        [SerializeField] private float maxEnergy = 100f;
        [SerializeField] private float energyRegen = 20f;
        [SerializeField] private float energyDecrease = 30f;
        [SerializeField] private StatusEffects statusEffect;
        [SerializeField] private int damageWhenNoEnergy = 1;
        [SerializeField] private LayerMask obstacleLayer;
        [SerializeField] private FlashSettings flashWhenPhasing = new FlashSettings(0.5f, 0.25f, 0, 1, new Color(0, 0.7176471f,1));
        [SerializeField] private float timeBetweenFlashes = 1f;

        private List<Collider> _colliders;
        private bool _applyingDamage;
        private float _currentEnergy;
        private float _statusEffectId;
        private Func<IEnumerator> _flashCached;
        private Coroutine _flashCoroutine;
        private WaitForSeconds _waitForNextFlash;
        private ImageFlash _imageFlash;

        public delegate void EnergyUpdateCallback(float energy);
        public event EnergyUpdateCallback OnEnergyUpdate;

        public float MaxEnergy => maxEnergy;

        private void Awake()
        {
            _currentEnergy = maxEnergy;
            _colliders = new List<Collider>(3);
            _flashCached = Flash;
            _waitForNextFlash = new WaitForSeconds(timeBetweenFlashes);
            var mainCamera = Camera.main;
            if (mainCamera != null) _imageFlash = mainCamera.GetComponent<ImageFlash>();
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
                    _flashCoroutine = StartCoroutine(_flashCached());
                }
                _colliders.Add(other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (1 << other.gameObject.layer != obstacleLayer || !_colliders.Contains(other)) return;
            _colliders.Remove(other);
            if (_colliders.Count > 0) return;
            StopCoroutine(_flashCoroutine);
            if (!_applyingDamage) return;
            statusEffect.RemoveStatusEffect(_statusEffectId);
            _applyingDamage = false;
        }

        private IEnumerator Flash()
        {
            while (true)
            {
                _imageFlash.Flash(flashWhenPhasing);
                yield return _waitForNextFlash;
            }
        }
    }
}
