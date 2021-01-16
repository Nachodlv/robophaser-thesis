using System;
using System.Collections;
using Cues;
using Photon.CustomPunPool;
using Photon.GameControllers;
using Photon.Pun;
using UnityEngine;
using Utils;

namespace Photon.Combat
{
    [Serializable]
    public class BulletCueByLayer
    {
        public LayerMask layer;
        public Cue cue;
    }
    public class Bullet : MonoBehaviourPun
    {
        [SerializeField] private int damage;
        [SerializeField] private float timeToLive;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Material[] materials;
        [SerializeField] private BulletCueByLayer[] bulletCueByLayer;

        private Rigidbody _rigidbody;
        private Coroutine _timeToLiveCoroutine;
        private readonly ContactPoint[] _hitContacts = new ContactPoint[5];
        private string _shootBy;
        private int _currentMaterial;

        private Rigidbody Rigidbody => _rigidbody != null ? _rigidbody : _rigidbody = GetComponent<Rigidbody>();

        private void Awake()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            _rigidbody = gameObject.AddComponent<Rigidbody>();
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            _rigidbody.useGravity = false;
        }

        private void OnEnable()
        {
            if (PhotonNetwork.IsMasterClient) _timeToLiveCoroutine = StartCoroutine(DestroyBulletCoroutine());
        }

        private void OnDisable()
        {
            if(PhotonNetwork.IsMasterClient) Rigidbody.velocity = Vector3.zero;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!PhotonNetwork.IsMasterClient) return;

            if (!CheckCollisionAndApplyDamage(other.gameObject, false)) return;

            if (other.GetContacts(_hitContacts) > 0)
            {
                ExecuteCue(other.gameObject, _hitContacts[0].point, _hitContacts[0].normal);
            }

            DestroyBullet();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!PhotonNetwork.IsMasterClient) return;
            if (!CheckCollisionAndApplyDamage(other.gameObject, true)) return;

            var position = transform.position;
            ExecuteCue(other.gameObject, position, position - other.transform.position);
        }

        public void Shoot(string userId, int playerNumber, Vector3 force)
        {
            _shootBy = userId;
            Rigidbody.AddForce(force, ForceMode.Impulse);
            if(_currentMaterial != playerNumber) photonView.RPC(nameof(RPC_SetMaterial), RpcTarget.All, playerNumber);
        }

        public void ExecuteCue(GameObject collidedWith, Vector3 contactPoint, Vector3 normal)
        {
            var rotation = Quaternion.LookRotation(normal);
            foreach (var cueByLayer in bulletCueByLayer)
            {
                if (cueByLayer.layer.Includes(collidedWith.layer))
                {
                    cueByLayer.cue.Execute(contactPoint, rotation);
                    return;
                }
            }
        }

        public void DestroyBullet()
        {
            StopCoroutine(_timeToLiveCoroutine);
            PunPool.Instance.Destroy(gameObject);
        }

        private IEnumerator DestroyBulletCoroutine()
        {
            yield return new WaitForSeconds(timeToLive);
            PunPool.Instance.Destroy(gameObject);
        }

        private bool CheckCollisionAndApplyDamage(GameObject collidedWith, bool isTrigger)
        {
            var avatarSetup = collidedWith.GetComponentInParent<AvatarSetup>();
            if (avatarSetup == null) return !isTrigger;

            if (avatarSetup.Id == _shootBy)
            {
                return false;
            }
            Debug.Log($"##### Hitting another player by {damage}");
            avatarSetup.TakeDamage(damage);
            return true;
        }

        [PunRPC]
        private void RPC_SetMaterial(int materialIndex)
        {
            meshRenderer.material = materials[materialIndex];
            _currentMaterial = materialIndex;
        }
    }
}
