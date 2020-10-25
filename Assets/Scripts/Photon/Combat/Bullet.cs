using System;
using Photon.Pun;
using UnityEngine;

namespace Photon.GameControllers
{
    public class Bullet : MonoBehaviourPun
    {
        [SerializeField] private float damage;
        [SerializeField] private float timeToLive;
        public event Action OnOpponentHit;

        private Rigidbody _rigidbody;
        public Rigidbody Rigidbody
        {
            get
            {
                if (_rigidbody == null) _rigidbody = GetComponentInChildren<Rigidbody>();
                return _rigidbody;
            }
        }

        private void Awake()
        {
            Invoke(nameof(OpponentHit), 5);
            if(photonView.IsMine) Invoke(nameof(DestroyBullet), timeToLive);
        }

        private void OpponentHit()
        {
            OnOpponentHit?.Invoke();
        }

        private void DestroyBullet()
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
