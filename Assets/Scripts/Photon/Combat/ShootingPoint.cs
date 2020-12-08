using UnityEngine;

namespace Photon.GameControllers
{
    public class ShootingPoint : MonoBehaviour
    {
        private Transform _transform;

        public Transform Transform => _transform != null ? _transform : (_transform = transform);
    }
}
