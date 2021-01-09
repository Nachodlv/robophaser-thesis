using Photon.Pun;
using UnityEngine;

namespace WFC
{
    [RequireComponent(typeof(PhotonView))]
    public class NetworkSimpleTileWFC : SimpleTiledWFC
    {
        [SerializeField] private float scale = 1f;

        private PhotonView _photonView;

        public float GridSize => gridsize * scale;

        private void Awake()
        {
            transform.localScale = new Vector3(scale, scale, scale);
            _photonView = GetComponent<PhotonView>();
        }

        protected override GameObject InstantiateGameObject(string prefabName, Vector3 position,
            Vector3 localEulerAngles)
        {
            var newGameObject = PhotonNetwork.Instantiate(prefabName.Substring(1, prefabName.Length - 1), Vector3.zero,
                Quaternion.identity);
            var newPhotonView = newGameObject.GetComponent<PhotonView>();
            _photonView.RPC(
                nameof(RPC_InitializeObstacle), RpcTarget.All, newPhotonView.ViewID, position, localEulerAngles);
            return newGameObject;
        }

        [PunRPC]
        private void RPC_InitializeObstacle(int obstacleViewId, Vector3 localPosition, Vector3 localEulerAngles)
        {

            var obstacle = PhotonView.Find(obstacleViewId);
            var parent = ObstacleParent;
            Transform obstacleTransform;
            (obstacleTransform = obstacle.transform).SetParent(parent);
            obstacleTransform.localPosition = localPosition;
            obstacleTransform.localEulerAngles = localEulerAngles;
            obstacleTransform.localScale = Vector3.one;
        }
    }
}
