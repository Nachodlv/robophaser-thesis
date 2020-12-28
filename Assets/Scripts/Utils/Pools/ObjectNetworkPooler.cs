using Photon.Pun;
using UnityEngine;

namespace Utils.Pools
{
    public class ObjectNetworkPooler <T>: ObjectPooler<T> where T : NetworkPooleable
    {
        private readonly string _prefabPath;

        public ObjectNetworkPooler(string prefabPath)
        {
            _prefabPath = prefabPath;
        }

        protected override T InstantiateObject(T objectToPool, Transform parent)
        {
            return PhotonNetwork.Instantiate(_prefabPath, Vector3.zero, Quaternion.identity).GetComponent<T>();
        }
    }
}
