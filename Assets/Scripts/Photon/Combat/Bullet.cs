using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace Photon.GameControllers
{
    public class Bullet : MonoBehaviourPun
    {
        [SerializeField] private int damage;
        [SerializeField] private float timeToLive;
        public event Action OnOpponentHit;

        private Rigidbody _rigidbody;
        private Coroutine _timeToLiveCoroutine;
        public Rigidbody Rigidbody => _rigidbody != null ? _rigidbody : _rigidbody = GetComponent<Rigidbody>();

        private void Awake()
        {
            Invoke(nameof(OpponentHit), 5);
            if (photonView.IsMine) _timeToLiveCoroutine = StartCoroutine(DestroyBullet());
        }

        private void OnCollisionEnter(Collision other)
        {
            StopCoroutine(_timeToLiveCoroutine);
            if (other.gameObject.TryGetComponent<PhotonPlayer>(out var photonPlayer))
            {
                photonPlayer.ReceiveDamage(damage);
            }
            PhotonNetwork.Destroy(gameObject);
        }

        private void OpponentHit()
        {
            OnOpponentHit?.Invoke();
        }

        private IEnumerator DestroyBullet()
        {
            yield return new WaitForSeconds(timeToLive);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
