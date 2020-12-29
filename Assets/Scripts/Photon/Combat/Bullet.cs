using System;
using System.Collections;
using Cues;
using Photon.Pun;
using UnityEngine;

namespace Photon.GameControllers
{
    public class Bullet : MonoBehaviourPun
    {
        [SerializeField] private int damage;
        [SerializeField] private float timeToLive;
        [SerializeField] private Cue cue;
        public event Action OnOpponentHit;

        private Rigidbody _rigidbody;
        private Coroutine _timeToLiveCoroutine;
        private ContactPoint[] _hitContacts = new ContactPoint[5];

        public Rigidbody Rigidbody => _rigidbody != null ? _rigidbody : _rigidbody = GetComponent<Rigidbody>();

        private void Awake()
        {
            Invoke(nameof(OpponentHit), 5);
            if (photonView.IsMine) _timeToLiveCoroutine = StartCoroutine(DestroyBullet());
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!PhotonNetwork.IsMasterClient) return;

            StopCoroutine(_timeToLiveCoroutine);
            if (other.gameObject.TryGetComponent<PhotonPlayer>(out var photonPlayer))
            {
                photonPlayer.ReceiveDamage(damage);
            }

            if (other.GetContacts(_hitContacts) > 0)
            {
                var contactPoint = _hitContacts[0];
                var rotation = Quaternion.LookRotation(contactPoint.normal);
                cue.Execute(contactPoint.point, rotation);
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
