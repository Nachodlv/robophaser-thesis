using System;
using Photon.Combat;
using Photon.GameControllers;
using Photon.Pun;
using UnityEngine;

namespace WFC
{
    public class MapLimits : MonoBehaviour
    {
        [SerializeField] private BoxCollider boxCollider;
        [SerializeField] private int damagePerSecondWhenOutOfLimits;
        [SerializeField] private float extraSpace = 1f;

        private float _currentStatusEffect;
        private bool _hasStatusEffect;

        public event Action LocalPlayerOutOfLimits;
        public event Action LocalPlayerInsideLimits;

        public void SetMapLimits(float gridSize, float width, float depth)
        {
            var size = boxCollider.size;
            size.x = gridSize * width + extraSpace;
            size.z = gridSize * depth + extraSpace;
            boxCollider.size = size;
            boxCollider.enabled = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (IsLocalPlayer(other, out var avatar))
            {
                if(_hasStatusEffect) return;

                _currentStatusEffect = avatar.StatusEffect.AddStatusEffect(damagePerSecondWhenOutOfLimits);
                _hasStatusEffect = true;
                LocalPlayerOutOfLimits?.Invoke();
                return;
            }

            if (!PhotonNetwork.IsMasterClient || !other.TryGetComponent<Bullet>(out var bullet)) return;

            var bulletPosition = bullet.transform.position;
            var bulletHeight = bulletPosition.y;
            var floorHeight = transform.position.y  + boxCollider.center.y - boxCollider.size.y / 2;
            if (bulletHeight > floorHeight) return;

            Debug.Log($"##### Bullet hitting map limit ({other.name})");
            bullet.ExecuteCue(gameObject, bulletPosition, Vector3.up);
            bullet.DestroyBullet();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsLocalPlayer(other, out var avatar)) return;
            if (!_hasStatusEffect) return;
            avatar.StatusEffect.RemoveStatusEffect(_currentStatusEffect);
            _hasStatusEffect = false;
            LocalPlayerInsideLimits?.Invoke();
        }

        private bool IsLocalPlayer(Collider other, out AvatarSetup avatarSetup)
        {
            avatarSetup = other.GetComponentInParent<AvatarSetup>();
            if (avatarSetup == null) return false;
            return avatarSetup.photonView.IsMine;
        }
    }
}
