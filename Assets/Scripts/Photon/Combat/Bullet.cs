using System;
using System.Collections;
using Cues;
using Photon.CustomPunPool;
using Photon.Pun;
using UnityEngine;

namespace Photon.GameControllers
{
    public class Bullet : MonoBehaviourPun
    {
        [SerializeField] private int damage;
        [SerializeField] private float timeToLive;
        [SerializeField] private Cue cue;

        private Rigidbody _rigidbody;
        private Coroutine _timeToLiveCoroutine;
        private ContactPoint[] _hitContacts = new ContactPoint[5];

        public Rigidbody Rigidbody => _rigidbody != null ? _rigidbody : _rigidbody = GetComponent<Rigidbody>();

        private void OnEnable()
        {
            if (PhotonNetwork.IsMasterClient) _timeToLiveCoroutine = StartCoroutine(DestroyBullet());
        }

        private void OnDisable()
        {
            if(PhotonNetwork.IsMasterClient) Rigidbody.velocity = Vector3.zero;
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
            PunPool.Instance.Destroy(gameObject);
        }


        private IEnumerator DestroyBullet()
        {
            yield return new WaitForSeconds(timeToLive);
            PunPool.Instance.Destroy(gameObject);
        }
    }
}
