using System.Collections;
using Cues;
using Photon.CustomPunPool;
using Photon.GameControllers;
using Photon.Pun;
using UnityEngine;

namespace Photon.Combat
{
    public class Bullet : MonoBehaviourPun
    {
        [SerializeField] private int damage;
        [SerializeField] private float timeToLive;
        [SerializeField] private Cue cue;

        private Rigidbody _rigidbody;
        private Coroutine _timeToLiveCoroutine;
        private readonly ContactPoint[] _hitContacts = new ContactPoint[5];
        private string _shootBy;

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
                ExecuteCue(_hitContacts[0].point, _hitContacts[0].normal);
            }

            DestroyBullet();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!PhotonNetwork.IsMasterClient) return;
            if (!CheckCollisionAndApplyDamage(other.gameObject, true)) return;

            var position = transform.position;
            ExecuteCue(position, position - other.transform.position);
            // var ray = new Ray(position, position - other.transform.position);
            // photonView.RPC(nameof(RPC_RaycastHelper), RpcTarget.All, position, position - other.transform.position);
            // if (Physics.Raycast(ray, out var hit))
            // {
            //     ExecuteCue(hit.point, hit.normal);
            // }
            DestroyBullet();
        }

        public void Shoot(string userId, Vector3 force)
        {
            _shootBy = userId;
            Rigidbody.AddForce(force, ForceMode.Impulse);
        }

        public void ExecuteCue(Vector3 contactPoint, Vector3 normal)
        {
            var rotation = Quaternion.LookRotation(normal);
            cue.Execute(contactPoint, rotation);
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
        private void RPC_RaycastHelper(Vector3 start, Vector3 direction)
        {
            Debug.DrawRay(start, direction, Color.red, 60);
        }
    }
}
