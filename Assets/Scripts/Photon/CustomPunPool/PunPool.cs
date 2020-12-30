using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using UnityEngine;
using Utils;

namespace Photon.CustomPunPool
{
    public class PooleableGroup
    {
        public List<PunPooleable> Pooleables;
        public int ParentViewId;
    }

    public class PunPool : Singleton<PunPool>, IPunPrefabPool
    {
        private Dictionary<string, PooleableGroup> _pool;


        protected override void Awake()
        {
            base.Awake();
            _pool = new Dictionary<string, PooleableGroup>();
        }

        public void CreateInstances(string prefabId, int quantity)
        {
            for (var i = 0; i < quantity; i++)
            {
                InstantiateNewGameObject(prefabId, Vector3.zero, Quaternion.identity,
                    _pool.TryGetValue(prefabId, out var pooleableGroup) ? pooleableGroup : null);
            }
        }

        public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
        {
            if (!PhotonNetwork.IsMasterClient) return null;

            if (_pool.TryGetValue(prefabId, out var pooleableGroup))
            {
                foreach (var pooleable in pooleableGroup.Pooleables)
                {
                    if (!pooleable.IsActive)
                    {
                        pooleable.SetPositionAndRotation(position, rotation);
                        pooleable.IsActive = true;
                        return pooleable.gameObject;
                    }
                }
            }

            return InstantiateNewGameObject(prefabId, position, rotation, pooleableGroup);
        }

        public void Destroy(GameObject gameObject)
        {
            foreach (var pooleableGroup in _pool.Values)
            {
                foreach (var pooleable in pooleableGroup.Pooleables)
                {
                    if (pooleable.gameObject == gameObject) pooleable.IsActive = false;
                }
            }
        }

        private static bool InstantiateParent(string prefabId, out int viewId)
        {
            var parent = PhotonNetwork
                .Instantiate(Path.Combine("Pool", "Pun Pool Parent"), Vector3.zero, Quaternion.identity).transform;
            if (parent.TryGetComponent<NetworkNameChanger>(out var nameChanger))
            {
                nameChanger.ChangeName($"Pool of {prefabId}");
            }

            if (parent.TryGetComponent<PhotonView>(out var photonView))
            {
                viewId = photonView.ViewID;
                return true;
            }

            viewId = default;
            return false;
        }

        private GameObject InstantiateNewGameObject(string prefabId, Vector3 position, Quaternion rotation,
            PooleableGroup pooleableGroup = null)
        {
            var currentPooleableGroup = pooleableGroup;
            if (currentPooleableGroup == null)
            {
                if (InstantiateParent(prefabId, out var parentId))
                {
                    pooleableGroup = new PooleableGroup
                        {ParentViewId = parentId, Pooleables = new List<PunPooleable>()};
                    _pool.Add(prefabId, pooleableGroup);
                }
                else
                {
                    return null;
                }
            }

            var newGameObject = PhotonNetwork.Instantiate(prefabId, position, rotation);
            if (newGameObject.TryGetComponent<PunPooleable>(out var punPooleable))
            {
                punPooleable.IsActive = true;
                punPooleable.SetParent(pooleableGroup.ParentViewId);
                pooleableGroup.Pooleables.Add(punPooleable);
            }

            return newGameObject;
        }
    }
}
